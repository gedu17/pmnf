using System;
using VidsNet.Enums;

namespace VidsNet.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public override string ActiveMenuItem
        {
            get
            {
                return "Virtual view";
            }
        }
    }
}