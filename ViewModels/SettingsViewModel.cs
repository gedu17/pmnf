
using System.Collections.Generic;
using VidsNet.Models;
using VidsNet.DataModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VidsNet.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {

        public bool IsAdmin {get;set;}
        public List<UserSetting> UserSettings {get;set;}
        public List<Setting> AdminSettings {get;set;}

        public TagBuilder DirectoryListing {get;set;}

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