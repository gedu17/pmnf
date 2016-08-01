using System.Security.Claims;
using System.Threading.Tasks;

namespace VidsNet.Interfaces
{
    public interface IUserRepository
    {
        bool ValidateLogin(string userName, string password);
        ClaimsPrincipal Get(string userName);

        Task ChangePassword(int userId, string password);
    }
}