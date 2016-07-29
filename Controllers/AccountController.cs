
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VidsNet.Interfaces;
using VidsNet.Models;

namespace VidsNet.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;
        private readonly ILogger _logger;
        public AccountController(IUserRepository userRepository, ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository;
            _logger = loggerFactory.CreateLogger("AccountController");
        }

        /*[HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }*/
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookie");
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if(!_userRepository.ValidateLogin(model.Username, model.Password)) {
                    return View(new LoginViewModel() { ErrorMessage = "Bad login info."});
                }

                await HttpContext.Authentication.SignInAsync("Cookie", _userRepository.Get(model.Username),
                new AuthenticationProperties{
                    ExpiresUtc = DateTime.Now.AddMinutes(120),
                    IsPersistent = true,
                    AllowRefresh = true
                });
                
                HttpContext.Session.SetString("Name", model.Username);
                //HttpContext.Session.SetInt32("Id", _userRepository.GetId(model.Username));
                //HttpContext.Session.SetInt32("Level", level);

                if(string.IsNullOrWhiteSpace(returnUrl)) {
                    return Redirect("/");
                }
                return Redirect(returnUrl);
            }
            return View(new LoginViewModel(){ ErrorMessage = "Unknown error."});
        }
    }
}