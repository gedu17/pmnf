using System.Collections.Generic;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Models;

namespace VidsNet.ViewModels
{
    public class PhysicalViewModel : BaseViewModel
    {

        public PhysicalViewModel(UserData userData) : base(userData) {
            VirtualItems = new List<BaseVirtualItem>();
            RealItems = new List<RealItem>();
            Paths = new Dictionary<int,string>();
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Physical view";
            }
        }

        public List<BaseVirtualItem> VirtualItems {get;set;}
        public Dictionary<int, string> Paths {get;set;} 
        public List<RealItem> RealItems {get;set;}
    }
}