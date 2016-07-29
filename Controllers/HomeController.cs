using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using VidsNet.Classes;
using VidsNet.Models;

namespace VidsNet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ILogger _logger;
        public HomeController(ILoggerFactory logger) {
            _logger = logger.CreateLogger("HomeController");
        }

        
        public IActionResult Index()
        {
            var model = new HomeViewModel();

            return View(model);
        }

        [Route("scan")]
        //TEST METHOD
        public IActionResult Scan() {
            var dict = new Dictionary<int, string>() { { 1, "/home/gedas/workspace/vidsnet/" }};
            var scanner = new VideoScanner(_logger, 1);
            scanner.ScanItems(dict);
            var result = "Video scan done.\r\n";

            var scanner2 = new SubtitleScanner(_logger, 1);
            scanner2.ScanItems(dict);

            return Content(result);
        }
    }
}