using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VidsNet.Scanners;
using System.Threading.Tasks;
using VidsNet.DataModels;
using VidsNet.Classes;
using VidsNet.Filters;
using VidsNet.Enums;

namespace VidsNet.Controllers
{
    [TypeFilter(typeof(ExceptionFilter))]
    [Authorize]
    public class ItemsController : BaseController
    {
        private ILogger _logger;
        private Scanner _scanner;
        private BaseDatabaseContext _db;
        public ItemsController(ILoggerFactory logger, Scanner scanner, UserData userData, BaseDatabaseContext db)
         : base(userData) {
            _logger = logger.CreateLogger("HomeController");
            _scanner = scanner;
            _db = db;
        }

        
        [HttpGet]
        public async Task<IActionResult> Scan() {
            if(ModelState.IsValid) {
                var data = await _scanner.Scan(_user.UserSettings.Where(x => x.Name == "path").OrderBy(x => x.Value).ToList());
                var newItemsHtml = string.Empty;
                if(data.NewItemsCount > 0 || data.DeletedItemsCount > 0) {
                    newItemsHtml = HtmlHelpers.GenerateScanResult(data);
                }

                if(data.NewItemsCount > 0) {
                    await _user.AddSystemMessage(string.Format("{0} items added.", data.NewItemsCount), Severity.Info, newItemsHtml); 
                }
                
                if(data.DeletedItemsCount > 0) {
                    await _user.AddSystemMessage(string.Format("{0} items removed.", data.DeletedItemsCount), Severity.Info, newItemsHtml);
                }            
                return Ok();
            }
            return NotFound();
        }
    }
}