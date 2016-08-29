using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using VidsNet.Models;
using VidsNet.Enums;
using System.Threading.Tasks;
using VidsNet.ViewModels;
using VidsNet.DataModels;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VidsNet.Classes
{
    public class UserRepository : BaseUserRepository
    {
        private List<User> _users;
        private DatabaseContext _db;
        public UserRepository(DatabaseContext db)
        {
            _db = db;
            _users = new List<User>();
            _users.AddRange(_db.Users.ToList());
        }

        public override bool ValidateLogin(string username, string password)
        {
            username = username.ToLower().Trim();
            password = password.Trim();
            var user = _users.Where(x => x.Name.ToLower() == username).FirstOrDefault();
            if (user is User)
            {
                var hash = Convert.FromBase64String(user.Password);
                var salt = GetSaltFromHashedPassword(hash);
                var hashedPassword = GetPasswordFromHashedPassword(hash);
                var checkedPassword = GetPasswordHash(password, salt);

                return PasswordsEqual(hashedPassword, checkedPassword);
            }
            else
            {
                return false;
            }
        }

        public override async Task<bool> ChangePassword(int userId, string password)
        {
            var user = _db.Users.Where(x => x.Id == userId).First();
            if (user is User)
            {
                user.Password = Convert.ToBase64String(CreatePasswordHash(password));
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public override List<User> GetUsers()
        {
            return _db.Users.ToList();
        }

        public override ClaimsPrincipal GetClaims(string userName)
        {
            var user = _users.FirstOrDefault(x => x.Name == userName);

            if (user == null)
            {
                return null;
            }

            var sessionHash = Guid.NewGuid().ToString();

            user.SessionHash = sessionHash;
            _db.Users.Update(user);
            _db.SaveChanges();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(Claims.Id.ToString(), user.Id.ToString(), ClaimValueTypes.String, Constants.Issuer));
            claims.Add(new Claim(Claims.Level.ToString(), user.Level.ToString(), ClaimValueTypes.String, Constants.Issuer));
            claims.Add(new Claim(Claims.Name.ToString(), user.Name.ToString(), ClaimValueTypes.String, Constants.Issuer));
            claims.Add(new Claim(Claims.SessionHash.ToString(), sessionHash, ClaimValueTypes.String, Constants.Issuer));

            var userIdentity = new ClaimsIdentity("VidsNet");
            userIdentity.AddClaims(claims);

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            return userPrincipal;

        }

        public override async Task<bool> SetActive(int userId, int value)
        {
            var user = _db.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user is User)
            {
                user.Active = value;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public override async Task<bool> SetAdmin(int userId, int value)
        {
            var user = _db.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user is User)
            {
                user.Level = value == 1 ? 9 : 1;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public override async Task<bool> CreateUser(NewUserViewModel user)
        {
            user.Name = user.Name.ToLower().Trim();
            if (!_db.Users.Any(x => x.Name.ToLower() == user.Name) && !string.IsNullOrWhiteSpace(user.Name)
            && !string.IsNullOrWhiteSpace(user.Password))
            {
                var newUser = new User()
                {
                    Name = user.Name,
                    Password = Convert.ToBase64String(CreatePasswordHash(user.Password.Trim())),
                    Level = user.Level > 1 ? 9 : 1,
                    Active = 1
                };
                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public override async Task<bool> DeleteUser(int id)
        {
            var user = _db.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user is User)
            {
                var userPaths = _db.UserSettings.Where(x => x.UserId == id && x.Name == "path").ToList();
                foreach (var userPath in userPaths)
                {
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

        public override byte[] GetSaltFromHashedPassword(byte[] password)
        {
            ;
            var salt = new byte[Constants.SaltSize];
            Buffer.BlockCopy(password, 0, salt, 0, Constants.SaltSize);

            return salt;
        }

        public override byte[] GetPasswordFromHashedPassword(byte[] password)
        {

            var hash = new byte[Constants.PasswordSize];
            Buffer.BlockCopy(password, Constants.SaltSize, hash, 0, Constants.PasswordSize);

            return hash;
        }

        public override byte[] CreatePasswordHash(string password)
        {
            var salt = GetSalt();
            var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, Constants.KeyDerivationIterationCount,
             Constants.PasswordSize);
            var ret = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, ret, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, ret, salt.Length, hash.Length);
            return ret;
        }
        public override byte[] GetPasswordHash(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, Constants.KeyDerivationIterationCount,
             Constants.PasswordSize);
        }

        public override byte[] GetSalt()
        {
            var salt = new byte[Constants.SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public override User GetUserBySessionHash(string hash)
        {
            return _users.Where(x => x.SessionHash == hash).FirstOrDefault();
        }
    }
}