using System.Collections.Generic;
using VidsNet.Classes;
using VidsNet.Models;

namespace VidsNet.ViewModels
{
    public class PhysicalViewModel : BaseViewModel
    {

        public PhysicalViewModel(UserData userData) : base(userData) {
            VirtualItems = new List<VirtualItem>();
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

        public List<VirtualItem> VirtualItems {get;set;}
        public Dictionary<int, string> Paths {get;set;} 
        public List<RealItem> RealItems {get;set;}
    }
}