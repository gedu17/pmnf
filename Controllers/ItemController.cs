
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VidsNet.DataModels;
using VidsNet.Models;
using VidsNet.Enums;
using VidsNet.Classes;

namespace VidsNet.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        private ILogger _logger;
        private DatabaseContext _db;
        private VideoViewer _viewer;
        public ItemController(ILoggerFactory logger, DatabaseContext db, UserData userData, VideoViewer viewer)
         : base(userData) {
            _logger = logger.CreateLogger("ItemsController");
            _db = db;            
            _viewer = viewer;
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
                UserId = _user.Id,
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
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id).SingleOrDefault();
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
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && x.IsDeleted == false).SingleOrDefault();
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
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && x.IsSeen == false).SingleOrDefault();
            if(item is VirtualItem) {
                item.SeenTime = DateTime.Now;
                item.IsSeen = true;
                _db.VirtualItems.Update(item);
                await _db.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult View(int id, string name) {
            if(ModelState.IsValid) {
                var result = _viewer.View(id, name, Request.Headers["Range"].FirstOrDefault());

                if(!string.IsNullOrWhiteSpace(result.ContentRange)) {
                    Response.Headers.Add("Content-Range", result.ContentRange);
                }

                Response.ContentLength = result.ContentLength;
                Response.StatusCode = result.StatusCode;

                return result.ActionResult;
            }

            return NotFound();
        }
    }
}