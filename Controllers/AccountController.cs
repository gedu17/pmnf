
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Filters;
using VidsNet.Interfaces;
using VidsNet.Models;
using VidsNet.Scanners;
using VidsNet.ViewModels;

namespace VidsNet.Controllers
{
    [TypeFilter(typeof(ExceptionFilter))]
    [Authorize]
    public class AccountController : BaseController
    {
        private BaseUserRepository _userRepository;
        private readonly ILogger _logger;
        private BaseScanner _baseScanner;
        private Scanner _scanner;
        public AccountController(BaseUserRepository userRepository, ILoggerFactory loggerFactory, UserData userData, BaseScanner baseScanner, Scanner scanner) 
         : base(userData)
        {
            _userRepository = userRepository;
            _logger = loggerFactory.CreateLogger("AccountController");
            _baseScanner = baseScanner;
            _scanner = scanner;
        }

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
            if(!_user.IsLoggedIn()){
                var model = new LoginViewModel(_user);
                model.ReturnUrl = returnUrl;
                return View(model);
            }
            return Redirect("/");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(!_userRepository.ValidateLogin(model.Username, model.Password)) {
                    return View(new LoginViewModel(_user) { ErrorMessage = "Bad login info."});
                }

                var claimsPrincipal = _userRepository.GetClaims(model.Username);
                await HttpContext.Authentication.SignInAsync("Cookie", claimsPrincipal,
                new AuthenticationProperties{
                    ExpiresUtc = DateTime.Now.AddMinutes(Constants.CookieExpiryTime),
                    IsPersistent = true,
                    AllowRefresh = true
                });

                _user.ParseClaims(claimsPrincipal.Claims);

                if(string.IsNullOrWhiteSpace(model.ReturnUrl)) {
                    return Redirect("/");
                }
                return Redirect(model.ReturnUrl);
            }
            return View(new LoginViewModel(_user){ ErrorMessage = "Unknown error."});
        }

        [HttpGet]
        public IActionResult Settings() {
            var homeDir = string.Empty;
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                homeDir = Environment.GetEnvironmentVariable("HOMEPATH");
            }
            else {
                homeDir = Environment.GetEnvironmentVariable("HOME");
            }

            var folders = _baseScanner.ScanItems(new List<string>() { homeDir }, new List<IScannerCondition>());
            var userPaths = _user.UserSettings.Where(x => x.Name == "path").ToList();
            var form = new TagBuilder("form");
            form.MergeAttribute("id", "userPathsForm");

            foreach(var folder in folders) {
                HtmlHelpers.GenerateDirectoryListing(folder, form, 1, userPaths);
            }
            var settings = new SettingsViewModel(_user) { 
                IsAdmin = _user.IsAdmin, 
                UserSettings = _user.UserSettings, 
                AdminSettings = _user.AdminSettings,
                DirectoryListing = form,
                Users = _userRepository.GetUsers() };
            return View(settings);
        }

        //Changes password
        [HttpPost]
        public async Task<IActionResult> Password([FromBody]PasswordViewModel settings) {
            if (ModelState.IsValid)
            {
                if(!_userRepository.ValidateLogin(_user.Name, settings.OldPassword))   {
                    return BadRequest();
                }
                if(await _userRepository.ChangePassword(_user.Id, settings.NewPassword)){
                    return Ok();
                }
                
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

        [HttpPut]
        public async Task<IActionResult> Active(int id, [FromBody]SettingsPostViewModel data) {
            if(ModelState.IsValid) {
                var value = int.Parse(data.Value);
                if(_user.IsAdmin && value >= 0 && value <= 1) {
                    if(await _userRepository.SetActive(id, value)){
                        return Ok(); 
                    }
                }
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Admin(int id, [FromBody]SettingsPostViewModel data) {
            if(ModelState.IsValid) {
                var value = int.Parse(data.Value);
                if(_user.IsAdmin && value >= 0 && value <= 1) {
                    if(await _userRepository.SetAdmin(id, value)){
                        return Ok(); 
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]NewUserViewModel user) {
            if(ModelState.IsValid) {
                if(_user.IsAdmin) {
                    if(await _userRepository.CreateUser(user)){
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult Users() {
            return PartialView("ManageUsers", new ManageUsersViewModel(_user) { Users = _userRepository.GetUsers()});
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            if(ModelState.IsValid && _user.IsAdmin) {
                if(await _userRepository.DeleteUser(id)){
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> UserPaths([FromBody]List<SettingsPostViewModel> paths) {
            if (ModelState.IsValid) {
                await _user.UpdateUserPaths(paths);
                var userPaths = _user.UserSettings.Where(x => x.Name == "path").OrderBy(x => x.Value).ToList();
                var data = await _scanner.Scan(userPaths);
                var newItemsHtml = string.Empty;
                if(data.NewItemsCount > 0 || data.DeletedItemsCount > 0) {
                    newItemsHtml = HtmlHelpers.GenerateScanResult(data);
                }

                if(data.NewItemsCount > 0) {
                    await _user.AddSystemMessage(string.Format("{0} items added.", data.NewItemsCount), Severity.Info, newItemsHtml); 
                }
                
                if(data.DeletedItemsCount > 0) {
                    await _user.AddSystemMessage(string.Format("{0} items removed.", data.DeletedItemsCount), Severity.Info, newItemsHtml);
                }
                
                return Ok();
            }
            return BadRequest();
        }
    }
}