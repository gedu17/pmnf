
using System;
using System.Collections.Generic;
using System.IO;
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
            foreach(var dir in dirs) {
                var dirId = AddRealItem(realParentId, userPathId, dir.FullName, ItemType.Folder);
                var virtDirId = AddVirtualItem(_userId, dirId, virtualParentId, dir.Name, ItemType.Folder);
                var item = new Item() { Id = dirId, Path = dir.FullName, Type = ItemType.Folder };
                _items.Add(item);
                
                Scan(dir.FullName, userPathId, dirId, virtDirId);
            }

            var files = di.GetFiles();
            foreach(var file in files) {
                if(IsCorrectType(file.Name)) {
                    var itemId = AddRealItem(realParentId, userPathId, file.FullName, ItemType.Video);
                    var virtItemId = AddVirtualItem(_userId, itemId, virtualParentId, file.Name, ItemType.Video);
                    var item = new Item() { Id = itemId, Path = file.FullName, Type = ItemType.Video };

                    _items.Add(item);
                }
            }
        }

        protected abstract bool IsCorrectType(string filePath);

        protected abstract int AddRealItem(int parentId, int userPathId, string path, ItemType type);
        protected abstract int AddVirtualItem(int userId, int realItemId, int parentId, string name, ItemType type);    

        public List<Item> ScanItems(Dictionary<int, string> userPaths) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be null");
            }

            foreach(var userPath in userPaths) {
                Scan(userPath.Value, userPath.Key);
            }
            
            return _items;
        }

    }

}