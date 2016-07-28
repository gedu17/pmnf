
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VidsNet
{
    public abstract class BaseScanner {
        protected ILogger _logger;
        private int _userId;
        public BaseScanner(ILogger logger, int userId) {
            _items = new List<Item>();
            _logger = logger;
            _userId = userId;
        }

        private List<Item> _items;
        private ILogger logger;

        private void Scan(string userPath, int userPathId, int realParentId = 0, int virtualParentId = 0) {
            _logger.LogDebug("Starting to scan " + userPath);
            var di = new DirectoryInfo(userPath);

            var dirs = di.GetDirectories();
            Parallel.ForEach(dirs, dir => {
                var dirId = AddRealItem(realParentId, userPathId, dir.FullName, ItemType.Folder).Result;
                var virtDirId = AddVirtualItem(_userId, dirId, virtualParentId, dir.Name, ItemType.Folder).Result;
                var item = new Item() { Id = dirId, Path = dir.FullName, Type = ItemType.Folder };
                _items.Add(item);
                
                Scan(dir.FullName, userPathId, dirId, virtDirId);
            });
            
            var files = di.GetFiles();
            Parallel.ForEach(files, file => {
                _logger.LogDebug("Found item " + file.Name);
                var check = CheckType(file.Name);
                if(check.CorrectType) {
                    var itemId = AddRealItem(realParentId, userPathId, file.FullName, check.Type).Result;

                    if(check.WriteVirtualItem) {
                        var virtItemId = AddVirtualItem(_userId, itemId, virtualParentId, file.Name, check.Type);
                    }
                    var item = new Item() { Id = itemId, Path = file.FullName, Type = check.Type };

                    _items.Add(item);
                }
            });
        }

        protected abstract CheckTypeResult CheckType(string filePath);

        protected async Task<int> AddRealItem(int parentId, int userPathId, string path, ItemType type)
        {
            using(var db = new DatabaseContext()) {
                if(!db.RealItems.Any(x => x.Path == path)) {
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
                    db.RealItems.Add(realItem);
                    //TODO: check if it doesnt make it worse
                    await db.SaveChangesAsync();
                    return realItem.Id;
                }
                else {
                    var results = db.RealItems.Where(x => x.Path == path).ToList();
                    return results[0].Id;
                }
                
            }
        }

        protected async Task<int> AddVirtualItem(int userId, int realItemId, int parentId, string name, ItemType type)
        {
            using(var db = new DatabaseContext()) {
                if(!db.VirtualItems.Any(x => x.UserId == userId && x.RealItemId == realItemId)) {
                    var virtualItem = new VirtualItem()
                    {
                        UserId = userId,
                        RealItemId = realItemId,
                        ParentId = parentId,
                        Type = type,
                        Name = name,
                        IsSeen = false,
                        IsDeleted = false
                    };
                    db.VirtualItems.Add(virtualItem);
                    await db.SaveChangesAsync();
                    return virtualItem.Id;
                }
                else {
                    var results = db.VirtualItems.Where(x => x.UserId == userId && x.RealItemId == realItemId).ToList();
                    return results[0].Id;
                }
                
            }
        }   

        public void ScanItems(Dictionary<int, string> userPaths) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }

            Parallel.ForEach(userPaths, userPath => Scan(userPath.Value, userPath.Key));           
        }
    }
}