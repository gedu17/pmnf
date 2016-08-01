
using System;
using Microsoft.AspNetCore.Http;
using VidsNet.Enums;

namespace VidsNet.Models
{
    public class LoginViewModel : BaseViewModel
    {

        public LoginViewModel() : base(null) {

        }

        public LoginViewModel(IHttpContextAccessor accessor) : base(accessor) {
            
        }

        public string Username {get;set;}
        public string Password {get;set;}
        public string ErrorMessage {get;set;}

        public override bool LoggedIn
        {
            get
            {
                return false;
            }
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Login";
            }
        }
    }
}