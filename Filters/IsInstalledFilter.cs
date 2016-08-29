using Microsoft.AspNetCore.Mvc.Filters;
using VidsNet.DataModels;
using VidsNet.ViewModels;

namespace VidsNet.Filters
{    
    public class IsInstalledFilter : IResourceFilter {

        private BaseUserRepository _userRepository;

        public IsInstalledFilter(BaseUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async void OnResourceExecuting(ResourceExecutingContext context)
        {
            if(_userRepository.GetUsers().Count == 0) {
                var user = new NewUserViewModel(){
                    Name = "admin",
                    Password = "admin",
                    Level = 9
                };
                await _userRepository.CreateUser(user);
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }
    }
}