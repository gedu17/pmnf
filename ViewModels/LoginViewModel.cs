using VidsNet.Classes;

namespace VidsNet.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        public LoginViewModel() : base(null) {

        }

        public LoginViewModel(UserData userData) : base(userData) {
            
        }

        public string Username {get;set;}
        public string Password {get;set;}
        public string ErrorMessage {get;set;}
        public string ReturnUrl {get;set;}

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