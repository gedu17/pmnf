

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Linq;


namespace VidsNet
{
    public class VideoScanner : BaseScanner
    {
        public VideoScanner(ILogger logger, int userId) : base(logger, userId) {
        }

        protected override int AddRealItem(int parentId, int userPathId, string path, ItemType type)
        {
            using(var db = new DatabseContext()) {
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
                    db.SaveChanges();
                    return realItem.Id;
                }
                else {
                    var results = db.RealItems.Where(x => x.Path == path).ToList();
                    return results[0].Id;
                }
                
            }
        }

        protected override int AddVirtualItem(int userId, int realItemId, int parentId, string name, ItemType type)
        {
            using(var db = new DatabseContext()) {
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
                    db.SaveChanges();
                    return virtualItem.Id;
                }
                else {
                    var results = db.VirtualItems.Where(x => x.UserId == userId && x.RealItemId == realItemId).ToList();
                    return results[0].Id;
                }
                
            }
        }

        protected override bool IsCorrectType(string filePath)
        {
            //TODO: implement using mime types or extensions
            return true;
        }
    }
}