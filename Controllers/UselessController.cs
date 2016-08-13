using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Models;

namespace VidsNet.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private ILogger _logger;
        private BaseDatabaseContext _db;
        public SettingsController(ILoggerFactory logger, BaseDatabaseContext db, UserData userData) 
        : base(userData) {
            _logger = logger.CreateLogger("Itemsontroller");
            _db = db;
        }

        public IActionResult Index() {
            return View();
        }
    }
}