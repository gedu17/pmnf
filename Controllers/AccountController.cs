
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;
using VidsNet.Models;
using VidsNet.Scanners;
using VidsNet.ViewModels;

namespace VidsNet.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private IUserRepository _userRepository;
        private readonly ILogger _logger;
        private BaseScanner _baseScanner;
        public AccountController(IUserRepository userRepository, ILoggerFactory loggerFactory, UserData userData, BaseScanner baseScanner) 
         : base(userData)
        {
            _userRepository = userRepository;
            _logger = loggerFactory.CreateLogger("AccountController");
            _baseScanner = baseScanner;
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

        private void Helper(Item item, TagBuilder select, int level, List<UserSetting> userPaths) {
            var padding = level * 25;
            foreach (var child in item.Children)
            {
                var guid = Guid.NewGuid().ToString();
                var div = new TagBuilder("div");
                div.AddCssClass("list-group-item");
                div.MergeAttribute("style", string.Format("padding-left: {0}px", padding));

                var checkbox = new TagBuilder("input");
                checkbox.MergeAttribute("type", "checkbox");
                checkbox.MergeAttribute("name", child.Path);
                checkbox.MergeAttribute("value", child.Path);

                if(userPaths.Any(x => x.Value == string.Format("{0}{1}", child.Path, Path.DirectorySeparatorChar))) {
                    checkbox.MergeAttribute("checked", "checked");
                }


                div.InnerHtml.AppendHtml(checkbox);

                if(child.Children.Count > 0) {
                    var button = new TagBuilder("button");
                    button.AddCssClass("btn");
                    button.AddCssClass("btn-link");
                    button.MergeAttribute("data-toggle", "collapse");
                    
                    button.MergeAttribute("data-target", string.Format("#{0}", guid));
                    button.MergeAttribute("type", "button");

                    var nameSpan = new TagBuilder("span");
                    nameSpan.InnerHtml.Append(child.Path);
                    nameSpan.MergeAttribute("id", string.Format("{0}_name", guid));
                    nameSpan.MergeAttribute("style", "margin-left: 15px;");

                    var buttonSpan = new TagBuilder("span");
                    buttonSpan.AddCssClass("glyphicon");
                    buttonSpan.AddCssClass("glyphicon-folder-open");
                    buttonSpan.MergeAttribute("aria-hidden", "true");
                    buttonSpan.InnerHtml.AppendHtml(nameSpan);

                    button.InnerHtml.AppendHtml(buttonSpan);
                    div.InnerHtml.AppendHtml(button);
                }
                else {
                    var nameSpan = new TagBuilder("span");
                    nameSpan.InnerHtml.Append(child.Path);
                    nameSpan.MergeAttribute("id", string.Format("{0}_name", guid));
                    nameSpan.MergeAttribute("style", "margin-left: 15px;");

                    var buttonSpan = new TagBuilder("span");
                    buttonSpan.AddCssClass("glyphicon");
                    buttonSpan.AddCssClass("glyphicon-folder-close");
                    buttonSpan.MergeAttribute("aria-hidden", "true");
                    buttonSpan.MergeAttribute("style", "padding: 6px 12px");
                    buttonSpan.InnerHtml.AppendHtml(nameSpan);
                    div.InnerHtml.AppendHtml(buttonSpan);
                }

                select.InnerHtml.AppendHtml(div);

                var collapsedDiv = new TagBuilder("div");
                collapsedDiv.MergeAttribute("id", guid);
                collapsedDiv.AddCssClass("collapse");
                

                if(child.Type == ItemType.Folder) {
                    Helper(child, collapsedDiv, level + 1, userPaths);
                }
                select.InnerHtml.AppendHtml(collapsedDiv);
            }
        }

        [HttpGet]
        public IActionResult Settings() {
            var home = Environment.GetEnvironmentVariable("HOME");
            var folders = _baseScanner.ScanItems(new List<string>() { home }, new List<IScannerCondition>());
            var userPaths = _user.UserSettings.Where(x => x.Name == "path").ToList();
            var form = new TagBuilder("form");
            form.MergeAttribute("id", "userPaths");

            foreach(var folder in folders) {
                Helper(folder, form, 1, userPaths);
            }
            var settings = new SettingsViewModel(_user) { 
                IsAdmin = _user.IsAdmin, 
                UserSettings = _user.UserSettings, 
                AdminSettings = _user.AdminSettings,
                DirectoryListing = form };
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