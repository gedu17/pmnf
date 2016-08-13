
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
        private Scanner _scanner;
        public AccountController(IUserRepository userRepository, ILoggerFactory loggerFactory, UserData userData, BaseScanner baseScanner, Scanner scanner) 
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
            var model = new LoginViewModel(_user);
            model.ReturnUrl = returnUrl;
            return View(model);
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

                var claimsPrincipal = _userRepository.Get(model.Username);
                await HttpContext.Authentication.SignInAsync("Cookie", claimsPrincipal,
                new AuthenticationProperties{
                    ExpiresUtc = DateTime.Now.AddMinutes(120),
                    IsPersistent = true,
                    AllowRefresh = true
                });

                if(string.IsNullOrWhiteSpace(model.ReturnUrl)) {
                    return Redirect("/");
                }
                return Redirect(model.ReturnUrl);
            }
            return View(new LoginViewModel(_user){ ErrorMessage = "Unknown error."});
        }
        //TODO: MOVE TO CSHTML
        private void Helper(Item item, TagBuilder select, int level, List<UserSetting> userPaths) {
            var padding = level * 25;
            foreach (var child in item.Children)
            {
                var pathValue = string.Format("{0}{1}", child.Path, Path.DirectorySeparatorChar);
                var guid = Guid.NewGuid().ToString();
                var div = new TagBuilder("div");
                div.AddCssClass("list-group-item");
                div.MergeAttribute("style", string.Format("padding-left: {0}px", padding));

                var checkbox = new TagBuilder("input");
                checkbox.MergeAttribute("type", "checkbox");
                checkbox.MergeAttribute("name", child.Path);
                checkbox.MergeAttribute("value", pathValue);
                checkbox.MergeAttribute("id", child.Id.ToString());

                if(userPaths.Any(x => x.Value == pathValue)) {
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
            form.MergeAttribute("id", "userPathsForm");

            foreach(var folder in folders) {
                Helper(folder, form, 1, userPaths);
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
                //TODO: REWORK TO PASS THE LIST TO METHOD?
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
                _logger.LogInformation("Username: " + user.Name + ", Password: " + user.Password + ", Level: " + user.Level);
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

                if(data.NewItemsCount > 0) {
                    await _user.AddSystemMessage(string.Format("{0} items added.", data.NewItemsCount), Severity.Info); 
                }
                
                if(data.DeletedItemsCount > 0) {
                    await _user.AddSystemMessage(string.Format("{0} items removed.", data.DeletedItemsCount), Severity.Info);
                }
                
                return Ok();
            }
            return BadRequest();
        }
    }
}