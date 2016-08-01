using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VidsNet.Models;

namespace VidsNet.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private ILogger _logger;
        private DatabaseContext _db;
        public SettingsController(ILoggerFactory logger, DatabaseContext db, IHttpContextAccessor accessor) 
        : base(accessor, db) {
            _logger = logger.CreateLogger("Itemsontroller");
            _db = db;
        }

        public IActionResult Index() {
            return View();
        }
    }
}