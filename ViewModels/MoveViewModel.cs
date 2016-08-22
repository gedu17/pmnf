using System.Collections.Generic;
using VidsNet.Models;
using VidsNet.Classes;
using Microsoft.AspNetCore.Mvc.Rendering;
using VidsNet.DataModels;

namespace VidsNet.ViewModels
{
    public class MoveViewModel : BaseViewModel
    {
        public List<BaseVirtualItem> Items {get;set;}
        public int CurrentItem {get;set;}

        public MoveViewModel(UserData userData) : base(userData) {
            Items = new List<BaseVirtualItem>();
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