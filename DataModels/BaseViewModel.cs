using Microsoft.AspNetCore.Http;
using VidsNet.Classes;
using VidsNet.Enums;
using System.Linq;
using VidsNet.DataModels;

namespace VidsNet.Models
{
    public abstract class BaseViewModel {
        public virtual bool LoggedIn { get{ return true; } }
        public abstract string ActiveMenuItem {get;}
        public string CurrentUrl {get;}

        public string PageTitle
        {
            get
            {
                var item = MenuItems.Items.Where(x => x.Url == CurrentUrl).FirstOrDefault();
                if(item is MenuItem) {
                    return item.Name;
                }
                return "";
            }
        }

        public MenuItems MenuItems {get;}

        public BaseViewModel(IHttpContextAccessor accessor) {
            MenuItems = new MenuItems();
            if(accessor != null) {
                CurrentUrl = accessor.HttpContext.Request.Path;
            }
            else {
                CurrentUrl = "/";
            }
        }
    }
}