
using System;
using VidsNet.Enums;

namespace VidsNet.Models
{
    public class LoginViewModel : BaseViewModel
    {
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

        //public bool RememberMe {get;set;}
    }
}