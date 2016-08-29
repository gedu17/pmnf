using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Models;

namespace VidsNet.Filters
{    
    public class UserLoggedInFilter : IResourceFilter {

        private UserData _userData;
        private BaseDatabaseContext _db;

        public UserLoggedInFilter(UserData userData, BaseDatabaseContext db)
        {
            _userData = userData;
            _db = db;
        }

        public async void OnResourceExecuting(ResourceExecutingContext context)
        {
            var userValid = false;
            var sessionHash = context.HttpContext.User.Claims.Where(x => x.Type == Claims.SessionHash.ToString()).FirstOrDefault();
            if(sessionHash is Claim) {
                var user = _db.Users.Where(x => x.SessionHash == sessionHash.Value).FirstOrDefault();
                if(user is User){
                    userValid = true;
                }                
            }
            
            if(!userValid) {
                if(context.HttpContext.User.Claims.Count() > 0) {
                    await context.HttpContext.Authentication.SignOutAsync("Cookie");
                    context.HttpContext.Session.Clear();
                    _userData.Dispose();
                    var urlHelper = new UrlHelper(context);
                    var loginUrl = new UrlRouteContext() { RouteName = "Login"};
                    context.HttpContext.Response.Redirect(urlHelper.RouteUrl(loginUrl));
                }
                
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }
    }
}