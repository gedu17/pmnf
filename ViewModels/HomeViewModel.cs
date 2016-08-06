using VidsNet.DataModels;

namespace VidsNet.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public HomeViewModel(UserData userData) : base(userData) {
            
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