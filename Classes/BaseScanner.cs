
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Models;

namespace VidsNet.Classes
{
    public abstract class BaseScanner {
        //IMPLEMENT THIS
        protected abstract CheckTypeResult CheckType(string filePath);


        protected ILogger _logger;
        private UserData _user;
        private DatabaseContext _db;
        private Object _lock;
        private List<Item> _items;
        private ILogger logger;
        private List<RealItem> _realItems;
        public BaseScanner(ILogger logger, IHttpContextAccessor accessor, DatabaseContext db){
            _db = db;
            _items = new List<Item>();
            _logger = logger;
            _user = new UserData(accessor.HttpContext.User, db);
            _lock = new Object();
            _realItems = db.RealItems.ToList();
        }
        private void Scan(string userPath, int userPathId, int realParentId = 0, int virtualParentId = 0) {
            _logger.LogDebug("Starting to scan " + userPath);
            var di = new DirectoryInfo(userPath);

            var dirs = di.GetDirectories();
            Parallel.ForEach(dirs, dir => {
                var dirId = AddRealItem(realParentId, userPathId, dir.FullName, ItemType.Folder);
                var virtDirId = AddVirtualItem(dirId, virtualParentId, dir.Name, ItemType.Folder);
                var item = new Item() { Id = dirId, Path = dir.FullName, Type = ItemType.Folder };
                _items.Add(item);
                
                Scan(dir.FullName, userPathId, dirId, virtDirId);
            });
            
            var files = di.GetFiles();
            Parallel.ForEach(files, file => {
                _logger.LogDebug("Found item " + file.Name);
                var check = CheckType(file.Name);
                if(check.CorrectType) {
                   
                    var itemId = AddRealItem(realParentId, userPathId, file.FullName, check.Type);

                    if(check.WriteVirtualItem) {
                        var virtItemId = AddVirtualItem(itemId, virtualParentId, file.Name, check.Type);
                    }

                    var item = new Item() { Id = itemId, Path = file.FullName, Type = check.Type };

                    _items.Add(item);
                }
            });
        }

        protected int AddRealItem(int parentId, int userPathId, string path, ItemType type)
        {
            lock(_lock) {
                if(!_db.RealItems.Any(x => x.Path == path)) {
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
        }
        protected int AddVirtualItem(int realItemId, int parentId, string name, ItemType type)
        {
            lock(_lock) {
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

        public List<Item> ScanItems(List<UserSetting> userPaths) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }

            Parallel.ForEach(userPaths, userPath => Scan(userPath.Value, userPath.Id));   

            return _items;        
        }
    }
}