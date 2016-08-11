using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VidsNet.Models;
using VidsNet.ViewModels;

namespace VidsNet.Interfaces
{
    public interface IUserRepository
    {
        bool ValidateLogin(string userName, string password);
        ClaimsPrincipal Get(string userName);
        Task ChangePassword(int userId, string password);
        List<User> GetUsers();
        Task<bool> SetActive(int userId, int value);
        Task<bool> SetAdmin(int userId, int value);
        Task<bool> CreateUser(NewUserViewModel user);
        Task<bool> DeleteUser(int id);
    }
}