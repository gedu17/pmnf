using System;
using VidsNet.Enums;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace VidsNet.Models
{
    public class HomeViewModel : BaseViewModel
    {

        public HomeViewModel(IHttpContextAccessor accessor) : base(accessor) {
            
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Virtual view";
            }
        }

        public string Body {get;set;}

        public object Data {get;set;}

        public object Data2 {get;set;}
    }
}