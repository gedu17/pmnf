using System;
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

        public MenuItems() {
            Items = new List<MenuItem>();
            Items.Add(new MenuItem(){ Name = "Virtual view", Url = "/" });
            Items.Add(new MenuItem(){ Name = "Physical view", Url = "/physical" });
            Items.Add(new MenuItem(){ Name = "Viewed view", Url = "/viewed" });
            Items.Add(new MenuItem(){ Name = "Scan", Url = "/scan" });
            Items.Add(new MenuItem(){ Name = "Settings", Url = "/account/settings" });
            Items.Add(new MenuItem(){ Name = "Logout", Url = "/account/logout" });
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