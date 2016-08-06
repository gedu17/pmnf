using VidsNet.DataModels;

namespace VidsNet.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {

        public ScanViewModel(UserData userData) : base(userData) {
            
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