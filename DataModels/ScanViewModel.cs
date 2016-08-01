using System;
using VidsNet.Enums;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace VidsNet.Models
{
    public class ScanViewModel : BaseViewModel
    {

        public ScanViewModel(IHttpContextAccessor accessor) : base(accessor) {
            
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Scan";
            }
        }

        public string Body {get;set;}

        public object Data {get;set;}
    }
}