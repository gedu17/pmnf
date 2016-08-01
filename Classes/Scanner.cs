using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VidsNet.Classes
{
    public class Scanner {
        private VideoScanner _videoScanner;
        private SubtitleScanner _subtitleScanner;
        private ILogger _logger;
        private DatabaseContext _db;
        public Scanner(ILoggerFactory logger, VideoScanner videoScanner, SubtitleScanner subtitleScanner, DatabaseContext db) {
            _db = db;
            _videoScanner = videoScanner;
            _subtitleScanner = subtitleScanner;
            _logger = logger.CreateLogger("Scanner");
        }

        public async Task<ScanResult> Scan(List<UserSetting> userPaths) {
            if(userPaths.Count == 0) {
                throw new ArgumentException("userPaths cannot be empty");
            }

            //TODO: use users paths ids !!!
            var oldItems = new List<Item>();
            await _db.RealItems.AsQueryable().ForEachAsync(x => oldItems.Add(new Item() {Path = x.Path, Id = x.Id, Type = x.Type }));
            var newItems = _videoScanner.ScanItems(userPaths);
            var subtitles = _subtitleScanner.ScanItems(userPaths);
            newItems = newItems.Union(subtitles).ToList();

            var result = GetItemChanges(oldItems, newItems);

            result.DeletedItems.ToList().ForEach(x => {
                
                var realItem = _db.RealItems.Where(y => y.Id == x.Id).FirstOrDefaultAsync().Result;
                if(realItem is RealItem) {
                    _db.RealItems.Remove(realItem);
                }

                var virtualItem = _db.VirtualItems.Where(y => y.RealItemId == x.Id).FirstOrDefaultAsync().Result;
                if(virtualItem is VirtualItem) {
                    _db.VirtualItems.Remove(virtualItem);
                }
            });

            //TODO: Maybe should add to db here aswell instead of basescanner????
            await _db.SaveChangesAsync();

            return result; 
        }


        private ScanResult GetItemChanges(List<Item> oldItems, List<Item> newItems) {
            var result = new ScanResult();

            if(oldItems.Count == 0) {
                result.NewItems.AddRange(newItems);
            }
            else {
                var difference = oldItems.Except(newItems).ToList();
                difference.AddRange(newItems.Except(oldItems).ToList());
                difference.ForEach(x =>  {
                    if(oldItems.Any(y => x == y) == true) {
                        result.DeletedItems.Add(x);
                    } 
                    else {
                        result.NewItems.Add(x);
                    }
                });
            }

            return result;
        }
    }
}