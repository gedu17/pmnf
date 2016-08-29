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
        private Scanner _scanner;
        private DatabaseContext _db;
        public ItemsController(Scanner scanner, UserData userData, DatabaseContext db)
         : base(userData)
        {
            _scanner = scanner;
            _db = db;
        }


        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            if (ModelState.IsValid)
            {
                var data = await _scanner.Scan(_user.UserSettings.Where(x => x.Name == "path").OrderBy(x => x.Value).ToList());
                var newItemsHtml = string.Empty;
                if (data.NewItemsCount > 0 || data.DeletedItemsCount > 0)
                {
                    newItemsHtml = HtmlHelpers.GenerateScanResult(data);
                }

                if (data.NewItemsCount > 0)
                {
                    await _user.AddSystemMessage(string.Format("{0} items added.", data.NewItemsCount), Severity.Info, newItemsHtml);
                }

                if (data.DeletedItemsCount > 0)
                {
                    await _user.AddSystemMessage(string.Format("{0} items removed.", data.DeletedItemsCount), Severity.Info, newItemsHtml);
                }
                return Ok();
            }
            return NotFound();
        }
    }
}