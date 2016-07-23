
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace VidsNet.Controllers
{
    public class RenameController : Controller
    {
        private ILogger _logger;
        public RenameController(ILoggerFactory logger) {
            _logger = logger.CreateLogger("RenameController");
        }
        public IActionResult Index()
        {
            return Content("Hello");
            
        }
    }
}