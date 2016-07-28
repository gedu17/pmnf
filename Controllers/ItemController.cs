
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace VidsNet.Controllers
{
    public class ItemController : Controller
    {
        private ILogger _logger;
        private int _userId;
        private DatabaseContext _db;
        public ItemController(ILoggerFactory logger, DatabaseContext db){//, int userId) {
            _logger = logger.CreateLogger("Itemsontroller");
            //TODO: fix userid!!!!
            _userId = 1;///userId;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]FrontEndItem frontEndItem) {
            /*{
                "name": "NewFolder",
                "Parent": 0
            }
            */
            var item = new VirtualItem()
            {
                UserId = _userId,
                RealItemId = 0,
                ParentId = frontEndItem.Parent,
                Name = frontEndItem.Name,
                IsSeen = false,
                IsDeleted = false,
                Type = ItemType.Folder
            };

            _db.VirtualItems.Add(item);
            await _db.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody]FrontEndItem frontEndItem) {
            /* {
                "name": "newName"
            }*/
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _userId).SingleOrDefault();
            if(item is VirtualItem) {
                item.Name = frontEndItem.Name;
                _db.VirtualItems.Update(item);
                await _db.SaveChangesAsync();
                return Ok();
            }
            
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _userId && x.IsDeleted == false).SingleOrDefault();
            if(item is VirtualItem) {
                item.DeletedTime = DateTime.Now;
                item.IsDeleted = true;
                _db.VirtualItems.Update(item);
                await _db.SaveChangesAsync();
                return Ok();
            }
            
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> Viewed(int id) {
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _userId && x.IsSeen == false).SingleOrDefault();
            if(item is VirtualItem) {
                item.SeenTime = DateTime.Now;
                item.IsSeen = true;
                _db.VirtualItems.Update(item);
                await _db.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        //TODO: IMEPLEMENT VIDEO AND SUBTITLE SERVING
        public async Task<IActionResult> view(int id, string name) {
            return Ok();
        }
    }
}