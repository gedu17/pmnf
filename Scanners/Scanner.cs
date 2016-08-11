using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Models;
using System.Linq;
using VidsNet.Enums;
using System.IO;
using VidsNet.Interfaces;

namespace VidsNet.Scanners
{
    public class Scanner {
        private BaseScanner _baseScanner;
        private ILogger _logger;
        private DatabaseContext _db;
        private Object _lock;
        private UserData _user;
        private ScanResult _scanResult;
        public Scanner(ILoggerFactory logger, BaseScanner baseScanner, DatabaseContext db, UserData userData) {
            _db = db;
            _baseScanner = baseScanner;
            _logger = logger.CreateLogger("Scanner");
            _lock = new Object();
            _user = userData;
            _scanResult = new ScanResult();
        }

        public async Task<ScanResult> Scan(List<UserSetting> userPaths) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }

            var oldItems = new List<Item>();
            var conditions = new List<IScannerCondition>() {new VideoCondition(), new SubtitleCondition()};
            var scannedItems = _baseScanner.ScanItems(userPaths, conditions);

            //TODO: PASS CONDITIONS TO GETCHILDREN
            foreach(var path in userPaths) {
                var item = new Item() {Path = path.Value, Type = ItemType.Folder, UserPathId = path.Id, Children = GetChildren(path.Id, 0) };
                oldItems.Add(item);
            }

            Action<Item> newItemsFunc = it => {
                _scanResult.NewItems.Add(it);
            };
            Action<Item> deletedItemsFunc = it => {
                _scanResult.DeletedItems.Add(it);
            };

            ItemDifferenceFinder(oldItems, scannedItems, newItemsFunc);
            ItemDifferenceFinder(scannedItems, oldItems, deletedItemsFunc);

            _scanResult.DeletedItems.ForEach(x => RemoveItems(x));
            _scanResult.NewItems.ForEach(x => AddItems(x, 0, 0));

            var orphinItems = _db.VirtualItems.Where(x => x.ParentId > 0 && !_db.VirtualItems.Any(y => y.Id == x.ParentId)).ToList();

            await _db.SaveChangesAsync();

            return _scanResult; 
        }

        private void ItemDifferenceFinder(List<Item> oldItems, List<Item> newItems, Action<Item> addFunction) {
            for(int i = 0; i < newItems.Count; i++) {
                if(oldItems.Count < (i+1)){
                    _scanResult.NewItems.Add(newItems[i]);
                }
                else {
                    for(int j = 0; j < newItems[i].Children.Count; j++) {
                        if(!oldItems[i].Children.Any(x => x.Path == newItems[i].Children[j].Path)) {
                            addFunction(newItems[i].Children[j]);
                        }
                        else if(newItems[i].Children[j].Type == ItemType.Folder) {
                            ItemDifferenceFinder(oldItems[i].Children[j].Children, newItems[i].Children[j].Children, addFunction);
                        }                        
                    }
                }
            }
        }

        private void AddItems(Item item, int realParentId, int virtualParentId) {

            if(realParentId == 0) {
                realParentId = AddRealItem(realParentId, item.UserPathId, item.Path, item.Type);
                if(item.WriteVirtualItem) {
                    virtualParentId = AddVirtualItem(realParentId, virtualParentId, item.Path, item.Type);
                }
            }

            foreach(var child in item.Children) {
                var realItem = AddRealItem(realParentId, child.UserPathId, child.Path, child.Type);
                var virtualItem = 0;
                if(child.WriteVirtualItem) {
                    virtualItem = AddVirtualItem(realItem, virtualParentId, child.Path, child.Type);
                }

                if(child.Type == ItemType.Folder) {
                    AddItems(child, realItem, virtualItem);
                }
            }
        }

        private void RemoveItems(Item item) {
            RealItem realItem;
            VirtualItem virtualItem;
            foreach(var child in item.Children) {
                if(child.Type == ItemType.Folder) {
                    RemoveItems(child);
                }
                else {
                    realItem = _db.RealItems.Where(x => x.Id == item.Id).FirstOrDefault();
                    if(realItem is RealItem) {
                        _db.RealItems.Remove(realItem);
                    }

                    virtualItem = _db.VirtualItems.Where(x => x.RealItemId == item.Id).FirstOrDefault();
                    if(virtualItem is VirtualItem) {
                        _db.VirtualItems.Remove(virtualItem);
                    }
                }
            }

            realItem = _db.RealItems.Where(x => x.Id == item.Id).FirstOrDefault();
            if(realItem is RealItem) {
                _db.RealItems.Remove(realItem);
            }

            virtualItem = _db.VirtualItems.Where(x => x.RealItemId == item.Id).FirstOrDefault();
            if(virtualItem is VirtualItem) {
                _db.VirtualItems.Remove(virtualItem);
            }
        }

        private List<Item> GetChildren(int userPathId, int parentId) {
            var ret = new List<Item>();
            var realItems = _db.RealItems.Where(x => x.ParentId == parentId && x.UserPathId == userPathId).OrderBy(x => x.Type).ToList();
            realItems = realItems.OrderBy(x => x.Path, System.StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach(var realItem in realItems) {
                var item = new Item() { Path = realItem.Path, Id = realItem.Id, Type = realItem.Type, UserPathId = realItem.UserPathId };
                if(realItem.Type == ItemType.Folder) {
                    item.Children.AddRange(GetChildren(userPathId, realItem.Id));
                }
                ret.Add(item);
            }
            return ret;
        }

        protected int AddRealItem(int parentId, int userPathId, string path, ItemType type)
        {
            if(!_db.RealItems.Any(x => x.Path == path && x.UserPathId == userPathId)) {
                string name = Path.GetFileName(path);
                var realItem = new RealItem()
                {
                    ParentId = parentId,
                    Type = type,
                    UserPathId = userPathId,
                    Name = name,
                    Path = path,
                    Extension = Path.GetExtension(path)
                };

                _db.RealItems.Add(realItem);
                _db.SaveChanges();
                return realItem.Id;
            }

            return _db.RealItems.FirstOrDefault(x => x.Path == path).Id;
        }
        protected int AddVirtualItem(int realItemId, int parentId, string name, ItemType type)
        {
            if(!_db.VirtualItems.Any(x => x.UserId == _user.Id && x.RealItemId == realItemId)) {
                var virtualItem = new VirtualItem()
                {
                    UserId = _user.Id,
                    RealItemId = realItemId,
                    ParentId = parentId,
                    Type = type,
                    Name = Path.GetFileNameWithoutExtension(name),
                    IsSeen = false,
                    IsDeleted = false
                };
                _db.VirtualItems.Add(virtualItem);
                _db.SaveChanges();
                return virtualItem.Id;
            }

            return _db.VirtualItems.FirstOrDefault(x => x.UserId == _user.Id && x.RealItemId == realItemId).Id;
        }   
    }
}