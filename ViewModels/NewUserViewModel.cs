
namespace VidsNet.ViewModels
{
    public class NewUserViewModel : BaseViewModel
    {

        public NewUserViewModel() : base(null) {

        }

        public string Name {get;set;}
        public string Password {get;set;}
        public int Level {get;set;}

        public override string ActiveMenuItem
        {
            get
            {
                return "Settings";
            }
        }
    }
}