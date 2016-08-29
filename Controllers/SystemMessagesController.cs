using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VidsNet.Scanners;
using System.Threading.Tasks;
using VidsNet.DataModels;
using VidsNet.Classes;
using VidsNet.Filters;
using VidsNet.Enums;
using VidsNet.ViewModels;
using VidsNet.Models;
using System.Collections.Generic;

namespace VidsNet.Controllers
{
    [TypeFilter(typeof(ExceptionFilter))]
    [Authorize]
    public class SystemMessagesController : BaseController
    {
        private ILogger _logger;
        private BaseDatabaseContext _db;
        public SystemMessagesController(ILoggerFactory logger, UserData userData, BaseDatabaseContext db)
         : base(userData) {
            _logger = logger.CreateLogger("SystemMessagesController");
            _db = db;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (ModelState.IsValid)
            {
                var messages = _db.SystemMessages.Where(x => x.UserId == _user.Id && x.Severity >= Severity.Info).OrderByDescending(y => y.Timestamp).ToList();
                //ICloneable interface seems to not exist in .NET CORE 1.0 so this is the workaround
                var oldMessages = new List<SystemMessage>();
                foreach (var message in messages)
                {
                    var sm = new SystemMessage() {
                        Id = message.Id,
                        UserId = message.Id,
                        Message = message.Message,
                        Read = message.Read,
                        Severity = message.Severity,
                        Timestamp = message.Timestamp,
                        LongMessage = message.LongMessage
                    };
                    oldMessages.Add(sm);
                }
                
                
                await _user.SetSystemMessagesAsRead();
                var model = new SystemMessagesViewModel(_user) { Messages = oldMessages };
                return View(model);
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Get(int id) {
            if(ModelState.IsValid) {
                var message = _db.SystemMessages.Where(x => x.UserId == _user.Id && x.Id == id).FirstOrDefault();
                if(message is SystemMessage) {
                    return Ok(message.LongMessage);
                }
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult GetCount() {
            if(ModelState.IsValid) {
                return Ok(_user.GetSystemMessageCount());
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Clean() {
            if(ModelState.IsValid) {
                await _user.CleanSystemMessages();
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            if(ModelState.IsValid) {
                if(await _user.DeleteSystemMessage(id)) {
                    return Ok();
                }
            }

            return NotFound();
        }
    }
}