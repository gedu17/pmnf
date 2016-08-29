using System.Collections.Generic;
using VidsNet.Classes;
using VidsNet.Models;

namespace VidsNet.ViewModels
{
    public class MoveViewModel : BaseViewModel
    {
        public List<VirtualItem> Items {get;set;}
        public int CurrentItem {get;set;}

        public MoveViewModel(UserData userData) : base(userData) {
            Items = new List<VirtualItem>();
            CurrentItem = 0;
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Virtual View";
            }
        }
    }
}