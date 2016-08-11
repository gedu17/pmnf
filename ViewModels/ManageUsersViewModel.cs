
using System.Collections.Generic;
using VidsNet.Models;
using VidsNet.DataModels;

namespace VidsNet.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        public List<User> Users {get;set;}

        public ManageUsersViewModel(UserData userData) : base(userData) {
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Manage users";
            }
        }
    }
}