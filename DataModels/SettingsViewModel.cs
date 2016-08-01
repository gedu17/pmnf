
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using VidsNet.Enums;

namespace VidsNet.Models
{
    public class SettingsViewModel : BaseViewModel
    {

        public bool IsAdmin {get;set;}
        public List<UserSetting> UserSettings {get;set;}
        public List<Setting> AdminSettings {get;set;}

        public SettingsViewModel() : base(null) {
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