using VidsNet.Classes;
using System.Linq;
using VidsNet.DataModels;

namespace VidsNet.ViewModels
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

        public BaseViewModel(UserData userData) {
            MenuItems = new MenuItems();
            if(userData != null) {
                CurrentUrl = userData.CurrentUrl;
            }
            else {
                CurrentUrl = "/";
            }
        }
    }
}