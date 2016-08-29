
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
            if(ModelState.IsValid) {
                if(Constants.IsSqlite) {
                    var item = new VirtualItemSqlite()
                    {
                        UserId = _user.Id,
                        RealItemId = null,
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
                        RealItemId = null,
                        ParentId = frontEndItem.Parent,
                        Name = frontEndItem.Name.Trim(),
                        IsViewed = false,
                        IsDeleted = false,
                        Type = Item.Folder
                    };
                    _db.VirtualItems.Add(item);
                    await _db.SaveChangesAsync();

                }

                await _user.AddSystemMessage("New item succesfully created.", Severity.Debug);
                return Ok();
                
            }
            
            await _user.AddSystemMessage("New item creation failed, modelstate is invalid.", Severity.Error);
            return NotFound();
        }
        
        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody]FrontEndItem frontEndItem) {
            /* {
                "name": "newName"
            }*/
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    item.Name = frontEndItem.Name.Trim();
                    _db.VirtualItems.Update(item);
                    await _db.SaveChangesAsync();
                    await _user.AddSystemMessage(string.Format("Item {0} succesfully edited with value {1}.", id, frontEndItem.Name), Severity.Debug);
                    return Ok();
                }
            }

            await _user.AddSystemMessage(string.Format("Item {0} editing with value {1} failed, modelstate is invalid.", id, frontEndItem.Name), Severity.Error);
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && !x.IsDeleted).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    item.DeletedTime = DateTime.Now;
                    item.IsDeleted = true;
                    _db.VirtualItems.Update(item);
                    await _db.SaveChangesAsync();
                    await _user.AddSystemMessage(string.Format("Item {0} succesfully flagged as deleted.", id), Severity.Debug);
                    return Ok();
                }
            }
            await _user.AddSystemMessage(string.Format("Item {0} flagging as deleted failed, id not found.", id), Severity.Error);
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> UnDelete(int id) {
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && x.IsDeleted).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    item.DeletedTime = DateTime.Now;
                    item.IsDeleted = false;
                    _db.VirtualItems.Update(item);
                    await _db.SaveChangesAsync();
                    await _user.AddSystemMessage(string.Format("Item {0} succesfully unflagged as deleted.", id), Severity.Debug);
                    return Ok();
                }
            }

            await _user.AddSystemMessage(string.Format("Item {0} unflagged as deleted failed, id not found.", id), Severity.Error);
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> Viewed(int id) {
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && !x.IsViewed).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    item.ViewedTime = DateTime.Now;
                    item.IsViewed = true;
                    _db.VirtualItems.Update(item);
                    await _db.SaveChangesAsync();
                    await _user.AddSystemMessage(string.Format("Item {0} succesfully flagged as viewed.", id), Severity.Debug);
                    return Ok();
                }
            }

            await _user.AddSystemMessage(string.Format("Item {0} flagging as viewed failed, id not found.", id), Severity.Error);
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> UnViewed(int id) {
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id && x.IsViewed).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    item.ViewedTime = DateTime.Now;
                    item.IsViewed = false;
                    _db.VirtualItems.Update(item);
                    await _db.SaveChangesAsync();
                    await _user.AddSystemMessage(string.Format("Item {0} succesfully unflagging as viewed.", id), Severity.Debug);
                    return Ok();
                }
            }

            await _user.AddSystemMessage(string.Format("Item {0} unflagging as viewed failed, id not found.", id), Severity.Error);
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> Move(int id, [FromBody]MoveItem move) {
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.Id == id && x.UserId == _user.Id).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    item.ParentId = move.ParentId;
                    _db.VirtualItems.Update(item);
                    await _db.SaveChangesAsync();
                    await _user.AddSystemMessage(string.Format("Item {0} succesfully moved to folder {1}.", id, move.ParentId), Severity.Debug);
                    return Ok();
                }
            }
            
            await _user.AddSystemMessage(string.Format("Item {0} moving to {1} failed, modelstate is invalid.", id, move.ParentId), Severity.Error);
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

            await _user.AddSystemMessage(string.Format("Item {0} for viewing with name {1} and session id {2} not found.", id, name, session), Severity.Error);
            return NotFound();
        }
    }
}