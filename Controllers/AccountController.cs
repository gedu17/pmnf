
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Interfaces;
using VidsNet.Models;
using VidsNet.ViewModels;

namespace VidsNet.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private IUserRepository _userRepository;
        private readonly ILogger _logger;
        public AccountController(IUserRepository userRepository, ILoggerFactory loggerFactory, UserData userData)
         : base(userData)
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
            return View(new LoginViewModel(_user));
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if(!_userRepository.ValidateLogin(model.Username, model.Password)) {
                    return View(new LoginViewModel(_user) { ErrorMessage = "Bad login info."});
                }

                await HttpContext.Authentication.SignInAsync("Cookie", _userRepository.Get(model.Username),
                new AuthenticationProperties{
                    ExpiresUtc = DateTime.Now.AddMinutes(120),
                    IsPersistent = true,
                    AllowRefresh = true
                });

                if(string.IsNullOrWhiteSpace(returnUrl)) {
                    return Redirect("/");
                }
                return Redirect(returnUrl);
            }
            return View(new LoginViewModel(_user){ ErrorMessage = "Unknown error."});
        }
        [HttpGet]
        public IActionResult Settings() {
            var settings = new SettingsViewModel() { IsAdmin = _user.IsAdmin, UserSettings = _user.UserSettings, AdminSettings = _user.AdminSettings };
            return View(settings);
        }

        //Changes password
        [HttpPost]
        public async Task<IActionResult> Settings([FromBody]PasswordViewModel settings) {
            if (ModelState.IsValid)
            {
                if(!_userRepository.ValidateLogin(_user.Name, settings.OldPassword ))   {
                    return BadRequest();
                }
                await _userRepository.ChangePassword(_user.Id, settings.NewPassword);
                return Ok();
            }
            return BadRequest();
        }
        //Updates user settings
        [HttpPost]
        public async Task<IActionResult> UserSettings([FromBody]List<SettingsPostViewModel> userSettings) {
            if (ModelState.IsValid)
            {
                foreach(var item in userSettings) {
                    await _user.UpdateSetting(item);
                }
                return Ok();
            }
            return BadRequest();
        }
        //Updates app settings
        [HttpPost]
        public async Task<IActionResult> AdminSettings([FromBody]List<SettingsPostViewModel> adminSettings) {
            if (ModelState.IsValid)
            {
                foreach(var item in adminSettings) {
                    await _user.UpdateAdminSetting(item);
                }
                return Ok();
            }
            return BadRequest();
        }

    }
}