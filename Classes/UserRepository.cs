using System;
using System.Collections.Generic;
using System.Security.Claims;
using VidsNet.Interfaces;
using System.Linq;
using VidsNet.Models;
using VidsNet.Enums;
using System.Threading.Tasks;
using VidsNet.ViewModels;

namespace VidsNet.Classes
{
    public class UserRepository : IUserRepository
    {
        private List<User> _users;
        private DatabaseContext _db;
        public UserRepository(DatabaseContext db) {
            _db = db;
            _users = new List<User>();
            _users.AddRange(_db.Users.ToList());
        }

        bool IUserRepository.ValidateLogin(string userName, string password)
        {
            if(_users.Exists(x => x.Name == userName && x.Password == password)) {
                return true;
            }
            else {
                return false;
            }
        }

        async Task IUserRepository.ChangePassword(int userId, string password) {
            var user = _db.Users.Where(x => x.Id == userId).First();
            user.Password = password;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        List<User> IUserRepository.GetUsers() {
            return _db.Users.ToList();
        }

        ClaimsPrincipal IUserRepository.Get(string userName)
        {
            var user = _users.FirstOrDefault(x => x.Name == userName);

            if(user == null) {
                return null;
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(Claims.Id.ToString(), user.Id.ToString(), ClaimValueTypes.String, Constants.Issuer));
            claims.Add(new Claim(Claims.Level.ToString(), user.Level.ToString(), ClaimValueTypes.String, Constants.Issuer));
            claims.Add(new Claim(Claims.Name.ToString(), user.Name.ToString(), ClaimValueTypes.String, Constants.Issuer));

            var userIdentity = new ClaimsIdentity("VidsNet");
            userIdentity.AddClaims(claims);

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            return userPrincipal;

        }

        async Task<bool> IUserRepository.SetActive(int userId, int value)
        {
            var user = _db.Users.Where(x => x.Id == userId).FirstOrDefault();
            if(user is User) {
                user.Active = value;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        async Task<bool> IUserRepository.SetAdmin(int userId, int value)
        {
            var user = _db.Users.Where(x => x.Id == userId).FirstOrDefault();
            if(user is User) {
                user.Level = value == 1 ? 9 : 1;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        async Task<bool> IUserRepository.CreateUser(NewUserViewModel user)
        {
            if(!_db.Users.Any(x => x.Name == user.Name) && !string.IsNullOrWhiteSpace(user.Name) 
            && !string.IsNullOrWhiteSpace(user.Password)) {
                var newUser = new User() { Name = user.Name, Password = user.Password, Level = user.Level > 1 ? 9 : 1, Active = 1 };
                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        async Task<bool> IUserRepository.DeleteUser(int id) {
            var user = _db.Users.Where(x => x.Id == id).FirstOrDefault();
            if(user is User) {
                var userPaths = _db.UserSettings.Where(x => x.UserId == id && x.Name == "path").ToList();
                foreach(var userPath in userPaths) {
                    var realItems = _db.RealItems.Where(x => x.UserPathId == userPath.Id).ToList();
                    _db.RealItems.RemoveRange(realItems);
                }
                var virtualItems = _db.VirtualItems.Where(x => x.UserId == id).ToList();
                _db.VirtualItems.RemoveRange(virtualItems);

                var userSettings = _db.UserSettings.Where(x => x.UserId == id).ToList();
                _db.UserSettings.RemoveRange(userSettings);

                var systemMessages = _db.SystemMessages.Where(x => x.UserId == id).ToList();
                _db.SystemMessages.RemoveRange(systemMessages);

                _db.Users.Remove(user);

                await _db.SaveChangesAsync();
                return true;
            }
            
            return false;
        }
    }
}