
using System;
using Microsoft.AspNetCore.Http;
using VidsNet.Enums;

namespace VidsNet.Models
{
    public class PasswordViewModel : BaseViewModel
    {

        public PasswordViewModel() : base(null) {

        }

        public string OldPassword {get;set;}
        public string NewPassword {get;set;}
        public string ErrorMessage {get;set;}

        public override string ActiveMenuItem
        {
            get
            {
                return "Settings";
            }
        }
    }
}