using System.Security.Claims;

namespace VidsNet.Interfaces
{
    public interface IUserRepository
    {
        bool ValidateLogin(string userName, string password);
        ClaimsPrincipal Get(string userName);
    }
}