using System.Collections.Generic;
using VidsNet.Classes;
using VidsNet.Models;

namespace VidsNet.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public HomeViewModel(UserData userData) : base(userData) {
            VirtualItems = new List<VirtualItem>();
            RealItems = new List<RealItem>();
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Virtual view";
            }
        }

        public List<VirtualItem> VirtualItems {get;set;}
        public List<RealItem> RealItems {get;set;}
        public object Data {get;set;}
    }
}