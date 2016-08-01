using System;
using System.Collections.Generic;
using System.Security.Claims;
using VidsNet.Interfaces;
using System.Linq;
using VidsNet.Models;
using VidsNet.Enums;
using System.Threading.Tasks;

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
    }
}