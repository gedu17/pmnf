using System.Collections;
using System.Collections.Generic;
using VidsNet.DataModels;

namespace VidsNet.Classes
{
    public class MenuItems : IEnumerator,IEnumerable
    {
        
        public List<MenuItem> Items {get; private set;}
        private int position = -1;

        object IEnumerator.Current
        {
            get
            {
                return Items[position];
            }
        }

        public MenuItems(int systemMessageCount) {
            var messageIcon = string.Empty;
            if(systemMessageCount > 0) {
                messageIcon = "messageIcon"; 
            }

            Items = new List<MenuItem>();
            Items.Add(new MenuItem(){ Name = "Virtual view", Url = "/", Html = "<i class=\"glyphicon glyphicon-facetime-video iconSmall\"></i>",
                Id = "virtual_link" });
            Items.Add(new MenuItem(){ Name = "Physical view", Url = "/physical", Html = "<i class=\"glyphicon glyphicon-th-list iconSmall\"></i>",
                Id = "physical_link" });
            Items.Add(new MenuItem(){ Name = "Settings", Url = "/account/settings", Html = "<i class=\"glyphicon glyphicon-wrench iconSmall\"></i>",
                Id = "settings_link" });
            Items.Add(new MenuItem(){ Name = "System Messages", Url = "/systemmessages", Id = "messages_link",
                Html = string.Format("<i class=\"glyphicon glyphicon-envelope iconSmall {0}\"></i>", messageIcon), Badge = systemMessageCount });
            Items.Add(new MenuItem(){ Name = "Logout", Url = "/account/logout", Html = "<i class=\"glyphicon glyphicon-log-out iconSmall\"></i>",
                Id = "logout_link" });
        }

        bool IEnumerator.MoveNext()
        {
            position++;
            return (position < Items.Count);
        }

        void IEnumerator.Reset()
        {
            position = 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this;
        }
    } 
}