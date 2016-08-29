
using System.Collections.Generic;
using VidsNet.Models;
using VidsNet.Classes;

namespace VidsNet.ViewModels
{
    public class SystemMessagesViewModel : BaseViewModel
    {
        public List<SystemMessage> Messages {get;set;}
        public int ListingType {get;set;}
        public SystemMessagesViewModel(UserData userData) : base(userData) {
            Messages = new List<SystemMessage>();
            ListingType = 0;
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "System Messages";
            }
        }
    }
}