using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VidsNet.ViewModels;
using VidsNet.DataModels;
using VidsNet.Classes;
using VidsNet.Filters;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VidsNet.Controllers
{
    [TypeFilter(typeof(ExceptionFilter))]
    [Authorize]
    public class HomeController : BaseController
    {
        private DatabaseContext _db;
        public HomeController(UserData userData, DatabaseContext db)
         : base(userData)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var virtualItems = await _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToListAsync();
            //Workaround to force EF load RealItem objects into VirtualItems
            var realItems = await _db.RealItems.ToListAsync();

            var model = new HomeViewModel(_user) { VirtualItems = virtualItems };
            return View(model);
        }

        [Route("physical")]
        public IActionResult Physical()
        {
            var virtualItems = _db.VirtualItems.Where(x => x.UserId == _user.Id).OrderBy(x => x.Type).ThenBy(y => y.Name).ToList();
            var paths = _user.UserSettings.Where(x => x.Name == "path").ToDictionary(y => y.Id, y => y.Value);
            var model = new PhysicalViewModel(_user) { VirtualItems = virtualItems, RealItems = _db.RealItems.ToList(), Paths = paths };
            return View("Physical", model);
        }

        [Route("error")]
        public IActionResult Error()
        {
            return Error();
        }
    }
}