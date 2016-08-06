using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VidsNet.DataModels;
using VidsNet.Models;

namespace VidsNet.Controllers
{
    public class BaseController : Controller {
        
        protected UserData _user;

        public BaseController(UserData user) {
            _user = user;
        }
        
    }
}