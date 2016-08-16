
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
using VidsNet.Interfaces;
using VidsNet.Filters;

namespace VidsNet.Controllers
{
    [Authorize]
    [TypeFilter(typeof(ExceptionFilter))]
    public class ItemController : BaseController
    {
        private ILogger _logger;
        private BaseDatabaseContext _db;
        private VideoViewer _viewer;
        private BaseUserRepository _userRepository;
        public ItemController(ILoggerFactory logger, BaseDatabaseContext db, UserData userData, VideoViewer viewer, BaseUserRepository userRepository)
         : base(userData) {
            _logger = logger.CreateLogger("ItemsController");
            _db = db;            
            _viewer = viewer;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]FrontEndItem frontEndItem) {
            /*{
                "name": "NewFolder",
                "Parent": 0
            }
            */
            if(Constants.IsSqlite) {
                var item = new VirtualItemSqlite()
                {
                    UserId = _user.Id,
                    RealItemId = 0,
                    ParentId = frontEndItem.Parent,
                    Name = frontEndItem.Name.Trim(),
                    IsViewed = false,
                    IsDeleted = false,
                    Type = Item.Folder
                };

                _db.VirtualItems.Add(item);
                await _db.SaveChangesAsync();
            }
            else {
                var item = new VirtualItem()
                {
                    UserId = _user.Id,
                    RealItemId = 0,
                    ParentId = frontEndItem.Parent,
                    Name = frontEndItem.Name.Trim(),
                    IsViewed = false,
                    IsDeleted = false,
                    Type = Item.Folder
                };
                _db.VirtualItems.Add(item);
                await _db.SaveChangesAsync();

            }

            
            return Ok();
        }
        
        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody]FrontEndItem frontEndItem) {
            /* {
                "name": "newName"
            }*/
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id).SingleOrDefault();
            if(item is BaseVirtualItem) {
                item.Name = frontEndItem.Name.Trim();
                _db.VirtualItems.Update(item);
                await _db.SaveChangesAsync();
                return Ok();
            }
            
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && x.IsDeleted == false).SingleOrDefault();
            if(item is BaseVirtualItem) {
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
            var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && x.IsViewed == false).SingleOrDefault();
            if(item is BaseVirtualItem) {
                item.ViewedTime = DateTime.Now;
                item.IsViewed = true;
                _db.VirtualItems.Update(item);
                await _db.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("item/view/{session}/{id}/{name}")]
        public async Task<IActionResult> View(Guid session, int id, string name) {
            if(ModelState.IsValid) {
                var user = _userRepository.GetUserBySessionHash(session.ToString());
                if(user is User) {
                    var result = await _viewer.View(id, user.Id, name, Request.Headers["Range"].FirstOrDefault());

                    if(!string.IsNullOrWhiteSpace(result.ContentRange)) {
                        Response.Headers.Add("Content-Range", result.ContentRange);
                    }

                    Response.ContentLength = result.ContentLength;
                    Response.StatusCode = result.StatusCode;

                    return result.ActionResult;
                }
            }

            return NotFound();
        }
    }
}