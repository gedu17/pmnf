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
    public class UserData : IDisposable
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public bool IsAdmin { get; private set; }
        public string CurrentUrl { get; private set; }
        public string SessionHash { get; private set; }

        private DatabaseContext _db;

        public List<UserSetting> UserSettings { get; private set; }
        public List<Setting> AdminSettings { get; private set; }

        public UserData(IHttpContextAccessor accessor, DatabaseContext db)
        {
            var principal = accessor.HttpContext.User;
            if (accessor.HttpContext.Request.Path.HasValue)
            {
                CurrentUrl = accessor.HttpContext.Request.Path.Value;
            }

            _db = db;
            AdminSettings = new List<Setting>();
            IsAdmin = false;
            Id = 0;

            ParseClaims(principal.Claims);
        }

        public bool IsLoggedIn()
        {
            return Id > 0;
        }

        public async Task UpdateAdminSetting(SettingsPostViewModel item)
        {
            if (IsAdmin)
            {
                var setting = _db.Settings.Where(x => x.Id == item.Id).FirstOrDefault();
                if (setting is Setting)
                {
                    setting.Value = item.Value.Trim();
                    _db.Settings.Update(setting);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateUserPaths(List<SettingsPostViewModel> paths)
        {
            var currentPaths = _db.UserSettings.Where(x => x.Name == "path").ToList();
            var newPaths = new List<UserSetting>();
            foreach (var path in paths)
            {
                var setting = new UserSetting() { Name = "path", UserId = Id, Value = path.Value, Description = "User path" };
                newPaths.Add(setting);
            }

            var itemsToAdd = newPaths.Except(currentPaths).ToList();
            var itemsToRemove = currentPaths.Except(newPaths).ToList();

            itemsToRemove.ForEach(x =>
            {
                var realItems = _db.RealItems.Where(y => y.UserPathId == x.Id).ToList();
                var virtualItems = new List<VirtualItem>();
                realItems.ForEach(y =>
                {
                    var virtualItem = _db.VirtualItems.Where(z => z.RealItemId == y.Id).FirstOrDefault();
                    if (virtualItem is VirtualItem)
                    {
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

        public async Task AddSystemMessage(string message, Severity severity, string longMessage = "")
        {
            var msg = new SystemMessage()
            {
                Message = message,
                Read = 0,
                Severity = severity,
                UserId = Id,
                Timestamp = DateTime.Now,
                LongMessage = longMessage
            };

            _db.SystemMessages.Add(msg);
            await _db.SaveChangesAsync();
        }

        public async Task SetSystemMessagesAsRead()
        {
            var messages = _db.SystemMessages.Where(x => x.UserId == Id).ToList();
            foreach (var message in messages)
            {
                message.Read = 1;
            }
            _db.SystemMessages.UpdateRange(messages);
            await _db.SaveChangesAsync();
        }

        public int GetSystemMessageCount()
        {
            var messages = _db.SystemMessages.Where(x => x.UserId == Id && x.Read == 0 && x.Severity >= Severity.Info).ToList();
            return messages.Count;
        }

        public async Task<bool> DeleteSystemMessage(int id)
        {
            var msg = _db.SystemMessages.Where(x => x.Id == id && x.UserId == Id).FirstOrDefault();
            if (msg is SystemMessage)
            {
                _db.SystemMessages.Remove(msg);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task CleanSystemMessages()
        {
            var messages = _db.SystemMessages.Where(x => x.UserId == Id).ToList();
            _db.SystemMessages.RemoveRange(messages);
            await _db.SaveChangesAsync();
        }

        public void ParseClaims(IEnumerable<Claim> claims)
        {
            var id = claims.FirstOrDefault(x => x.Type == Claims.Id.ToString());
            if (id is Claim)
            {
                Id = Int32.Parse(id.Value);
            }
            var name = claims.FirstOrDefault(x => x.Type == Claims.Name.ToString());
            if (name is Claim)
            {
                Name = name.Value;
            }
            var level = claims.FirstOrDefault(x => x.Type == Claims.Level.ToString());
            if (level is Claim)
            {
                Level = Int32.Parse(level.Value);
                if (Level == 9)
                {
                    IsAdmin = true;
                    AdminSettings = _db.Settings.ToList();
                }
            }

            var sessionHash = claims.FirstOrDefault(x => x.Type == Claims.SessionHash.ToString());
            if (sessionHash is Claim)
            {
                SessionHash = sessionHash.Value;
            }

            UserSettings = _db.UserSettings.Where(x => x.UserId == Id).ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Id = 0;
                    Name = string.Empty;
                    Level = 0;
                    IsAdmin = false;
                    SessionHash = Guid.Empty.ToString();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}