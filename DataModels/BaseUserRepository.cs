using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using VidsNet.Models;
using VidsNet.ViewModels;

namespace VidsNet.DataModels
{
    public abstract class BaseUserRepository
    {
        public abstract bool ValidateLogin(string username, string password);
        public abstract ClaimsPrincipal Get(string userName);
        public abstract Task<bool> ChangePassword(int userId, string password);
        public abstract List<User> GetUsers();
        public abstract Task<bool> SetActive(int userId, int value);
        public abstract Task<bool> SetAdmin(int userId, int value);
        public abstract Task<bool> CreateUser(NewUserViewModel user);
        public abstract Task<bool> DeleteUser(int id);
        public abstract byte[] GetSaltFromHashedPassword(byte[] password);
        public abstract byte[] GetPasswordFromHashedPassword(byte[] password);
        public abstract byte[] GetPasswordHash(string password, byte[] salt);
        public abstract byte[] CreatePasswordHash(string password);
        public abstract byte[] GetSalt();
        public abstract User GetUserBySessionHash(string hash);

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        protected static bool PasswordsEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }

    }
}