using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VidsNet.DataModels;
using VidsNet.Models;

namespace VidsNet.Controllers
{
    public class BaseController : Controller {
        
        protected UserData _user;

        public BaseController(IHttpContextAccessor accessor, DatabaseContext db) : base() {
            _user = new UserData(accessor.HttpContext.User, db);
        }
        
    }
}