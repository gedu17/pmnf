
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;
using VidsNet.Models;

namespace VidsNet.Scanners
{
    public class BaseScanner {

        protected ILogger _logger;
        private BaseDatabaseContext _db;
        private Object _lock;
        private List<Item> _items;
        private List<RealItem> _realItems;

        public BaseScanner(ILoggerFactory logger, BaseDatabaseContext db, UserData userData){ 
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

        private List<ScanItem> Scan(string userPath, int userPathId, List<IScannerCondition> conditions, bool ignoreHiddenFiles){
            var di = new DirectoryInfo(userPath);
            var dirs = di.GetDirectories();
            var items = new List<ScanItem>();
            var item = new ScanItem() { Path = userPath, Type = Item.Folder, UserPathId = userPathId };
            
            foreach (var dir in dirs)
            {
                if(!IsHiddenItem(dir.Name, ignoreHiddenFiles)){ 
                    var tmp = Scan(dir.FullName, userPathId, conditions, ignoreHiddenFiles);
                    item.Children.AddRange(tmp);
                }    
            }
            
            var files = di.GetFiles();
            foreach(var file in files) {
                if(!IsHiddenItem(file.Name, ignoreHiddenFiles)){ 
                    foreach(var condition in conditions) {
                        var check = condition.CheckType(file.Name);
                        if(check.CorrectType) {
                            item.Children.Add(new ScanItem() { Path = file.FullName, Type = check.Type, UserPathId = userPathId, 
                            WriteVirtualItem = check.WriteVirtualItem });
                            break;
                        }
                    }
                }
            }

            items.Add(item);
            return items;
        }

        public List<ScanItem> ScanItems(List<UserSetting> userPaths, List<IScannerCondition> conditions, bool ignoreHiddenFiles = true) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }

            var items = new List<ScanItem>(); 
            foreach(var userPath in userPaths) {
                items.AddRange(Scan(userPath.Value, userPath.Id, conditions, ignoreHiddenFiles));
            }

            return Sort(items);        
        }

        public List<ScanItem> ScanItems(List<string> userPaths, List<IScannerCondition> conditions, bool ignoreHiddenFiles = true) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }
            var items = new List<ScanItem>();
            foreach (var userPath in userPaths)
            {
                items.AddRange(Scan(userPath, 0, conditions, ignoreHiddenFiles));
            }

            return Sort(items);        
        }

        public List<ScanItem> Sort(List<ScanItem> items) {
            if(items.Count > 0) {
                items = items.OrderBy(x => x.Type).ThenBy(x => x.Path).ToList();
                for(int i = 0; i < items.Count; i++) {
                    items[i].Children = items[i].Children.OrderBy(x => x.Type).ThenBy(x => x.Path).ToList();
                    for(int j = 0; j < items[i].Children.Count; j++) {
                        items[i].Children[j].Children = Sort(items[i].Children[j].Children);
                    }
                }
            }
            
            
            return items;
        }
    }
}