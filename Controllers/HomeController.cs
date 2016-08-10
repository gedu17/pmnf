using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VidsNet.Scanners;
using VidsNet.Models;
using System.Threading.Tasks;
using VidsNet.ViewModels;
using VidsNet.DataModels;
using System;

namespace VidsNet.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private ILogger _logger;
        private Scanner _scanner;
        private DatabaseContext _db;
        public HomeController(ILoggerFactory logger, Scanner scanner, UserData userData, DatabaseContext db)
         : base(userData) {
            _logger = logger.CreateLogger("HomeController");
            _scanner = scanner;
            _db = db;
        }

        
        public IActionResult Index()
        {
            var data =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
            var data2 = _db.RealItems.ToList();
            var model = new HomeViewModel(_user) {Data = data, Data2 = data2 };
            return View(model);
        }

        [Route("scan")]
        //TEST METHOD
        public async Task<IActionResult> Scan() {
            var set = await _scanner.Scan(_user.UserSettings.Where(x => x.Name == "path").ToList());            
            var model = new ScanViewModel(_user) {Data = set};
            return View(model);
        }
    }
}