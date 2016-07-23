
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace VidsNet.Controllers
{
    public class RealItemsController : Controller
    {
        private ILogger _logger;
        public RealItemsController(ILoggerFactory logger) {
            _logger = logger.CreateLogger("RealItemsController");
        }
        public IActionResult Index()
        {
            return Content("Hello");
            
        }
    }
}