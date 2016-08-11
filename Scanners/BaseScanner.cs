
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;
using VidsNet.Models;

namespace VidsNet.Scanners
{
    public class BaseScanner {

        protected ILogger _logger;
        private DatabaseContext _db;
        private Object _lock;
        private List<Item> _items;
        private List<RealItem> _realItems;

        public BaseScanner(ILoggerFactory logger, DatabaseContext db, UserData userData){ 
            _db = db;
            _items = new List<Item>();
            _logger = logger.CreateLogger("BaseScanner");
            _lock = new Object();
            _realItems = db.RealItems.ToList();
        }

        private bool IsHiddenItem(string name, bool ignoreHiddenFiles) {

            if(!ignoreHiddenFiles) {
                return false;
            }

            if(name[0] == '.') {
                return true;
            }

            return false;
        }

        private List<Item> Scan(string userPath, int userPathId, List<IScannerCondition> conditions, bool ignoreHiddenFiles){
            _logger.LogDebug("Starting to scan " + userPath);
            var di = new DirectoryInfo(userPath);
            var dirs = di.GetDirectories();
            var items = new List<Item>();
            var item = new Item() { Path = userPath, Type = ItemType.Folder, UserPathId = userPathId };
            Parallel.ForEach(dirs, dir => {
                if(!IsHiddenItem(dir.Name, ignoreHiddenFiles)){ 
                    item.Children.AddRange(Scan(dir.FullName, userPathId, conditions, ignoreHiddenFiles));
                }              
            });
            
            var files = di.GetFiles();
            Parallel.ForEach(files, file => {
                if(!IsHiddenItem(file.Name, ignoreHiddenFiles)){ 
                    _logger.LogDebug("Found item " + file.Name);
                    foreach(var condition in conditions) {
                        var check = condition.CheckType(file.Name);
                        if(check.CorrectType) {
                            item.Children.Add(new Item() { Path = file.FullName, Type = check.Type, UserPathId = userPathId, 
                            WriteVirtualItem = check.WriteVirtualItem });
                            break;
                        }
                    }
                }
            });
            items.Add(item);
            return items;
        }

        public List<Item> ScanItems(List<UserSetting> userPaths, List<IScannerCondition> conditions, bool ignoreHiddenFiles = true) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }

            var items = new List<Item>();
            Parallel.ForEach(userPaths, userPath => items.AddRange(Scan(userPath.Value, userPath.Id, conditions, ignoreHiddenFiles)));  

            return Sort(items);        
        }

        public List<Item> ScanItems(List<string> userPaths,List<IScannerCondition> conditions, bool ignoreHiddenFiles = true) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }
            var items = new List<Item>();
            Parallel.ForEach(userPaths, userPath => {
                var scannedItems = Scan(userPath, 0, conditions, ignoreHiddenFiles);
                if(scannedItems == null) {
                    _logger.LogCritical("SCANNED ITEMS = NULL");
                    items.Add(new Item() { Path = userPath, Type = ItemType.Folder, UserPathId = 0, WriteVirtualItem = false  });
                }
                else {
                    items.AddRange(scannedItems);
                }
            });   

            return Sort(items);        
        }

        public List<Item> Sort(List<Item> items) {
            if(items.Count > 0) {
                items = items.OrderBy(x => x.Type).ThenBy(x => x.Path).ToList();
                for(int i = 0; i < items.Count; i++) {
                    if(items[i] != null) {
                        items[i].Children = items[i].Children.OrderBy(x => x.Type).ThenBy(x => x.Path).ToList();
                        for(int j = 0; j < items[i].Children.Count; j++) {
                            items[i].Children[j].Children = Sort(items[i].Children[j].Children);
                        }
                    }
                }
            }
            
            
            return items;
        }
    }
}