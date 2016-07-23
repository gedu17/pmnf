using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace VidsNet.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger;
        public HomeController(ILoggerFactory logger) {
            _logger = logger.CreateLogger("HomeController");
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("Id");
            if(id.HasValue) {
                _logger.LogInformation("Id: " + id);
            }
            return View();
        }

        [Route("logout")]
        public void Logout()
        {
            var id = HttpContext.Session.GetInt32("Id");
            if(id.HasValue) {
                _logger.LogInformation("Logging out user with id " + id);
                HttpContext.Session.Clear();
            }
            
            Redirect("/");
        }

        [Route("login")]
        [HttpGet]
        public IActionResult Login() {
            return Content("LUL");
        }

        //[HttpPost]
        //public void Login([FromBody]string username, [FromBody]string password) {
        
        [Route("login")]
        [HttpPost]
        public void Login(int id){
            using(var db = new DatabseContext()) {
                if(db.Users.Any(x => x.Id == id)) {
                    var user = db.Users.Where(x => x.Id == id).ToList();
                    SetSession(user[0].Name, user[0].Id, user[0].Level);
                    Redirect("/");
                }
                else {
                    _logger.LogError(string.Format("User with id {0} not found.", id));
                }
            }
        }

        private void SetSession(string name, int id, int level) {
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetInt32("Id", id);
            HttpContext.Session.SetInt32("Level", level);
        }
    }
}