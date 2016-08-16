using Microsoft.AspNetCore.Mvc;
using VidsNet.Classes;

namespace VidsNet.Controllers
{
    public class BaseController : Controller {
        
        protected UserData _user;

        public BaseController(UserData user) {
            _user = user;
        }
        
    }
}