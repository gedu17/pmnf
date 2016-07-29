using VidsNet.Classes;
using VidsNet.Enums;

namespace VidsNet.Models
{
    public abstract class BaseViewModel {
        public virtual bool LoggedIn { get{ return true; } }
        public abstract string ActiveMenuItem {get;}

        public MenuItems MenuItems {get;}

        public BaseViewModel() {
            MenuItems = new MenuItems();
        }
    }
}