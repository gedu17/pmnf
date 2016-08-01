using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using VidsNet.Classes;
using VidsNet.Models;
using System.Threading.Tasks;

namespace VidsNet.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private ILogger _logger;
        private Scanner _scanner;
        private IHttpContextAccessor _accessor;
        private DatabaseContext _db;
        public HomeController(ILoggerFactory logger, Scanner scanner, IHttpContextAccessor accessor, DatabaseContext db)
         : base(accessor, db) {
            _logger = logger.CreateLogger("HomeController");
            _scanner = scanner;
            _accessor = accessor;
            _db = db;
        }

        
        public IActionResult Index()
        {
            var data =  _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ToList();
            var data2 = _db.RealItems.ToList();
            var model = new HomeViewModel(_accessor) {Data = data, Data2 = data2 };

            return View(model);
        }

        [Route("scan")]
        //TEST METHOD
        public async Task<IActionResult> Scan() {
            var set = await _scanner.Scan(_user.UserSettings.Where(x => x.Name == "path").ToList());            
            var model = new ScanViewModel(_accessor) {Data = set};
            return View(model);
        }
    }
}