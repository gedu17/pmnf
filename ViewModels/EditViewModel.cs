using VidsNet.Classes;

namespace VidsNet.ViewModels
{
    public class EditViewModel : BaseViewModel
    {
        public string Value {get;set;}

        public EditViewModel(UserData userData) : base(userData) {
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