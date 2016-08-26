
using System.Collections.Generic;
using VidsNet.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using VidsNet.Classes;

namespace VidsNet.ViewModels
{
    public class SystemMessagesViewModel : BaseViewModel
    {
        public List<SystemMessage> Messages {get;set;}
        public SystemMessagesViewModel(UserData userData) : base(userData) {
            Messages = new List<SystemMessage>();
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