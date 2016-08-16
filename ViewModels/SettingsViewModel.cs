
using System.Collections.Generic;
using VidsNet.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using VidsNet.Classes;

namespace VidsNet.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {

        public bool IsAdmin {get;set;}
        public List<UserSetting> UserSettings {get;set;}
        public List<Setting> AdminSettings {get;set;}
        public TagBuilder DirectoryListing {get;set;}
        public List<User> Users {get;set;}
        public SettingsViewModel(UserData userData) : base(userData) {
            UserSettings = new List<UserSetting>();
            AdminSettings = new List<Setting>();
        }

        public override string ActiveMenuItem
        {
            get
            {
                return "Settings";
            }
        }
    }
}