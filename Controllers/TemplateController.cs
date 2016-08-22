using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Filters;
using VidsNet.ViewModels;

namespace VidsNet.Controllers
{
    [Authorize]
    [TypeFilter(typeof(ExceptionFilter))]
    public class TemplateController : BaseController
    {
        private ILogger _logger;
        private BaseDatabaseContext _db;
        public TemplateController(ILoggerFactory logger, BaseDatabaseContext db, UserData userData) 
        : base(userData) {
            _logger = logger.CreateLogger("TemplateController");
            _db = db;
        }

        public IActionResult Move(int id)
        {
            if(ModelState.IsValid) {
                var currentItem = _db.VirtualItems.Where(x => x.Id == id).FirstOrDefault();
                var currentItemId = 0;
                if(currentItem is BaseVirtualItem) {
                    currentItemId = currentItem.ParentId;
                }
                var viewModel = new MoveViewModel(_user) { Items = _db.VirtualItems.Where(x => x.UserId == _user.Id && !x.IsDeleted && !x.IsViewed).ToList(), 
                CurrentItem = currentItemId };
                return View(viewModel);
            }
            return NotFound();
        }

        public IActionResult VirtualItems() {
            if(ModelState.IsValid) {
                var virtualItems =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
                var realItems = _db.RealItems.ToList();
                var model = new HomeViewModel(_user) {VirtualItems = virtualItems, RealItems = realItems };
                return View(model);
            }
            return NotFound();
        }

        public IActionResult ViewedItems() {
            if(ModelState.IsValid) {
                var virtualItems =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
                var realItems = _db.RealItems.ToList();
                var model = new HomeViewModel(_user) {VirtualItems = virtualItems, RealItems = realItems };
                return View(model);
            }
            return NotFound();
        }

        public IActionResult DeletedItems() {
            if(ModelState.IsValid) {
                var virtualItems =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
                var realItems = _db.RealItems.ToList();
                var model = new HomeViewModel(_user) {VirtualItems = virtualItems, RealItems = realItems };
                return View(model);
            }
            return NotFound();
        }

        public IActionResult Edit(int id) {
            if(ModelState.IsValid) {
                var item = _db.VirtualItems.Where(x => x.UserId == _user.Id && x.Id == id).FirstOrDefault();
                if(item is BaseVirtualItem) {
                    var model = new EditViewModel(_user) { Value = item.Name };
                return View(model);
                }
            }

            return NotFound();
        }

        public IActionResult Create() {
            if(ModelState.IsValid) {
                var viewModel = new MoveViewModel(_user) { Items = _db.VirtualItems.Where(x => x.UserId == _user.Id && !x.IsDeleted && !x.IsViewed).ToList(), 
                CurrentItem = 0 };
                return View(viewModel);
            }

            return NotFound();
        }
    }
}