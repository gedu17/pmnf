using System;
using System.Security.Claims;
using VidsNet.Enums;
using System.Linq;
using System.Collections.Generic;
using VidsNet.ViewModels;
using VidsNet.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VidsNet.DataModels;

namespace VidsNet.Classes
{
    public class UserData {
        public int Id {get; private set;}
        public string Name {get; private set;}
        public int Level {get; private set;}
        public bool IsAdmin {get; private set;}
        public string CurrentUrl {get; private set;}
        public string SessionHash {get; private set;}

        private BaseDatabaseContext _db;

        public List<UserSetting> UserSettings {get; private set;}
        public List<Setting> AdminSettings {get; private set;}

        public UserData(IHttpContextAccessor accessor, BaseDatabaseContext db) {
            var principal = accessor.HttpContext.User;
            if(accessor.HttpContext.Request.Path.HasValue) {
                CurrentUrl = accessor.HttpContext.Request.Path.Value;
            }
            
            _db = db;
            AdminSettings = new List<Setting>();
            IsAdmin = false;
            Id = 0;
            
            ParseClaims(principal.Claims);
        }

        public bool IsLoggedIn() {
            return Id > 0;
        }

        public async Task UpdateAdminSetting(SettingsPostViewModel item) {
            if(IsAdmin) {
                var setting = _db.Settings.Where(x => x.Id == item.Id).FirstOrDefault();
                if(setting is Setting) {
                    setting.Value = item.Value.Trim();
                    _db.Settings.Update(setting);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateUserPaths(List<SettingsPostViewModel> paths) {
            var currentPaths = _db.UserSettings.Where(x => x.Name == "path").ToList();
            var newPaths = new List<UserSetting>();
            foreach (var path in paths)
            {
                var setting = new UserSetting() { Name = "path", UserId = Id, Value = path.Value, Description = "User path"};
                newPaths.Add(setting);
            }

            var itemsToAdd = newPaths.Except(currentPaths).ToList();
            var itemsToRemove = currentPaths.Except(newPaths).ToList();

            itemsToRemove.ForEach(x => {
                var realItems = _db.RealItems.Where(y => y.UserPathId == x.Id).ToList();
                var virtualItems = new List<BaseVirtualItem>();
                realItems.ForEach(y => {
                    var virtualItem = _db.VirtualItems.Where(z => z.RealItemId == y.Id).FirstOrDefault();
                    if(virtualItem is BaseVirtualItem) {
                        virtualItems.Add(virtualItem);
                    }
                });
                _db.RealItems.RemoveRange(realItems);
                _db.VirtualItems.RemoveRange(virtualItems);
            });

            _db.UserSettings.RemoveRange(itemsToRemove);
            _db.UserSettings.AddRange(itemsToAdd);

            await _db.SaveChangesAsync();
            UserSettings = _db.UserSettings.Where(x => x.UserId == Id).ToList();

        }

        public async Task AddSystemMessage(string message, Severity severity) {
            var msg = new SystemMessage() {
                Message = message,
                Read = 0,
                Severity = severity,
                UserId = Id,
                Timestamp = DateTime.Now
            };

            _db.SystemMessages.Add(msg);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SetSystemMessageAsRead(int id) {
            var msg = _db.SystemMessages.Where(x => x.Id == id && x.UserId == Id).FirstOrDefault();
            if(msg is SystemMessage) {
                msg.Read = 1;
                _db.SystemMessages.Update(msg);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSystemMessage(int id) {
            var msg = _db.SystemMessages.Where(x => x.Id == id && x.UserId == Id).FirstOrDefault();
            if(msg is SystemMessage) {
                _db.SystemMessages.Remove(msg);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task CleanSystemMessages() {
            var messages = _db.SystemMessages.Where(x => x.UserId == Id).ToList();
            _db.SystemMessages.RemoveRange(messages);
            await _db.SaveChangesAsync();
        }

        public void ParseClaims(IEnumerable<Claim> claims) {
            var id = claims.FirstOrDefault(x => x.Type == Claims.Id.ToString());
            if(id is Claim){
                Id = Int32.Parse(id.Value);
            }
            var name = claims.FirstOrDefault(x => x.Type == Claims.Name.ToString());
            if(name is Claim){
                Name = name.Value;
            }
            var level = claims.FirstOrDefault(x => x.Type == Claims.Level.ToString());
            if(level is Claim){
                Level = Int32.Parse(level.Value);
                if(Level == 9) {
                    IsAdmin = true;
                    AdminSettings = _db.Settings.ToList();
                }
            }

            var sessionHash = claims.FirstOrDefault(x => x.Type == Claims.SessionHash.ToString());
            if(sessionHash is Claim){
                SessionHash = sessionHash.Value;
            }

            UserSettings = _db.UserSettings.Where(x => x.UserId == Id).ToList();
        }
    }
}