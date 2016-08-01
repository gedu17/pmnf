using System;
using System.Security.Claims;
using VidsNet.Enums;
using System.Linq;
using System.Collections.Generic;
using VidsNet.Models;
using System.Threading.Tasks;

namespace VidsNet.DataModels
{
    public class UserData {
        public int Id {get; private set;}
        public string Name {get; private set;}
        public int Level {get; private set;}

        public bool IsAdmin {get; private set;}

        private DatabaseContext _db;

        public List<UserSetting> UserSettings {get; private set;}
        public List<Setting> AdminSettings {get; private set;}

        public UserData(ClaimsPrincipal principal, DatabaseContext db) {
            _db = db;
            AdminSettings = new List<Setting>();
            IsAdmin = false;
            var id = principal.Claims.FirstOrDefault(x => x.Type == Claims.Id.ToString());
            if(id is Claim){
                Id = Int32.Parse(id.Value);
            }
            var name = principal.Claims.FirstOrDefault(x => x.Type == Claims.Name.ToString());
            if(name is Claim){
                Name = name.Value;
            }
            var level = principal.Claims.FirstOrDefault(x => x.Type == Claims.Level.ToString());
            if(level is Claim){
                Level = Int32.Parse(level.Value);
                if(Level == 9) {
                    IsAdmin = true;
                    AdminSettings = db.Settings.ToList();
                }
            }

            UserSettings = db.UserSettings.Where(x => x.UserId == Id).ToList();
        }

        public async Task UpdateSetting(SettingsPostViewModel item) {
            var setting = _db.UserSettings.Where(x => x.Id == item.Id && x.UserId == Id).FirstOrDefault();
            if(setting is UserSetting) {
                setting.Value = item.Value;
                _db.UserSettings.Update(setting);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateAdminSetting(SettingsPostViewModel item) {
            if(IsAdmin) {
                var setting = _db.Settings.Where(x => x.Id == item.Id).FirstOrDefault();
                if(setting is Setting) {
                    setting.Value = item.Value;
                    _db.Settings.Update(setting);
                    await _db.SaveChangesAsync();
                }
            }
        }

    }
}