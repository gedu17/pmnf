using System;
using System.Collections.Generic;
using System.Security.Claims;
using VidsNet.Interfaces;
using System.Linq;
using VidsNet.Models;

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

        ClaimsPrincipal IUserRepository.Get(string userName)
        {
            var user = _users.FirstOrDefault(x => x.Name == userName);

            if(user == null) {
                return null;
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Default", userName, ClaimValueTypes.String, "localhost"));

            if(user.Level == 9) {
                claims.Add(new Claim("Administrator", userName, ClaimValueTypes.String, "localhost"));
            }

            var userIdentity = new ClaimsIdentity("VidsNet");
            userIdentity.AddClaims(claims);

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            return userPrincipal;

        }
    }
}