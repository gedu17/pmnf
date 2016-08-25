using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VidsNet.Scanners;
using VidsNet.ViewModels;
using VidsNet.DataModels;
using VidsNet.Classes;
using VidsNet.Filters;

namespace VidsNet.Controllers
{
    [TypeFilter(typeof(ExceptionFilter))]
    [Authorize]
    public class HomeController : BaseController
    {
        private ILogger _logger;
        private Scanner _scanner;
        private BaseDatabaseContext _db;
        public HomeController(ILoggerFactory logger, Scanner scanner, UserData userData, BaseDatabaseContext db)
         : base(userData) {
            _logger = logger.CreateLogger("HomeController");
            _scanner = scanner;
            _db = db;
        }

        
        public IActionResult Index()
        {
            if(Constants.IsSqlite) {
                var virtualItems =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
                var realItems = _db.RealItems.ToList();
                var model = new HomeViewModel(_user) {VirtualItems = virtualItems, RealItems = realItems };
                return View(model);
            }
            else {
                var virtualItems =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
                var model = new HomeViewModel(_user) {VirtualItems = virtualItems};
                return View(model);
            }
            
        }

        [Route("physical")]
        public IActionResult Physical() {
            var virtualItems =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
            var paths = _user.UserSettings.Where(x => x.Name == "path").ToDictionary(y => y.Id, y => y.Value);
            var model = new PhysicalViewModel(_user) {VirtualItems = virtualItems, RealItems = _db.RealItems.ToList(), Paths = paths};
            return View("Physical", model);
        }

        [Route("error")]
        public IActionResult Error() {
            return Ok("Error occured!");
        }
    }
}