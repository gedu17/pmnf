using System.Collections.Generic;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Models;

namespace VidsNet.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public HomeViewModel(UserData userData) : base(userData) {
            VirtualItems = new List<BaseVirtualItem>();
            RealItems = new List<RealItem>();
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Virtual view";
            }
        }

        public List<BaseVirtualItem> VirtualItems {get;set;}
        public List<RealItem> RealItems {get;set;}
        public object Data {get;set;}
    }
}