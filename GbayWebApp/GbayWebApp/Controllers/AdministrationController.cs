using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using GbayWebApp.Models;
using GbayWebApp.ViewModels;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.Extensions.Configuration;

namespace GbayWebApp.Controllers
{

    [Authorize(Roles = "Administrators")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;

        public AdministrationController(RoleManager<AppRole> roleManager,
                                        UserManager<AppUser> userManager,
                                        IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole identityRole = new AppRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            var roles = roleManager.Roles;
            var users = userManager.Users;
            ListRolesViewModel model = new ListRolesViewModel();
            foreach (AppRole role in roles)
            {
                ListRolesIndivudalViewModel vm = new ListRolesIndivudalViewModel();
                vm.Id = role.Id;
                vm.Name = role.Name;
                foreach (AppUser user in users)
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        vm.Users.Add(user);
                    }
                }
                model.Roles.Add(vm);
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id.ToString());

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            DeleteRoleViewModel model = new DeleteRoleViewModel();
            var role = await roleManager.FindByIdAsync(id);
            model.Id = role.Id;
            model.RoleName = role.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(DeleteRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.Id.ToString());
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Administration");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var UserRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id.ToString(),
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    UserRoleViewModel.IsSelected = true;
                }
                else
                {
                    UserRoleViewModel.IsSelected = false;
                }

                model.Add(UserRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });

        }

        [HttpGet]
        public async Task<IActionResult> ListUsersAsync()
        {
            List<AppUser> users = userManager.Users.ToList();
            List<ListUsersViewModel> modelList = new List<ListUsersViewModel>(users.Count);

            foreach (var user in users)
            {
                ListUsersViewModel vm = new ListUsersViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = await userManager.GetRolesAsync(user)
                };
                modelList.Add(vm);
            }
            return View(modelList);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                SecQuestion1 = user.SecurityQuestion1,
                SecQuestion2 = user.SecurityQuestion2
            };
            var roles = roleManager.Roles.ToList();
            var rolesUserIsIn = await userManager.GetRolesAsync(user);
            var RoleList = new List<EditUserRoleViewModel>();
            for (int i=0; i < roles.Count(); i++)
            {
                var vm = new EditUserRoleViewModel();
                vm.RoleName = roles[i].Name;
                if (rolesUserIsIn.Contains(roles[i].Name))
                {
                    vm.IsSelected = true;
                }
                else
                {
                    vm.IsSelected = false;
                }
                RoleList.Add(vm);
            }
            model.Roles = RoleList;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model, string id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "User Id not found";
                return View("Error");
            }

            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                if (user.Email != model.Email)
                {
                    user.Email = model.Email;
                    user.EmailConfirmed = false;

                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                           new { userId = user.Id, token = token }, Request.Scheme);

                    SmtpClient client = new SmtpClient();
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate(configuration["EmailUsernameSecret"], configuration["EmailPasswordsecret"]);

                    MimeMessage message = new MimeMessage();
                    MailboxAddress from = new MailboxAddress("Grant Shanklin", "grantshanklintest@gmail.com");
                    message.From.Add(from);
                    MailboxAddress to = new MailboxAddress(user.UserName, user.Email);
                    message.To.Add(to);
                    message.Subject = "Confirm Email";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = $"Please confirm your email by clicking on this link: {confirmationLink}";
                    message.Body = bodyBuilder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();

                }
                user.SecurityQuestion1 = model.SecQuestion1;
                user.SecurityQuestion2 = model.SecQuestion2;

                await userManager.UpdateAsync(user);

                return RedirectToAction("ListUsers", "Administration");
            }

            return View(model);
            
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            ViewBag.username = user.UserName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRoles(EditUserViewModel model, string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                for (int i = 0; i < model.Roles.Count; i++)
                {
                    var role = await roleManager.FindByNameAsync(model.Roles[i].RoleName);

                    IdentityResult result;

                    if (model.Roles[i].IsSelected && !await userManager.IsInRoleAsync(user, model.Roles[i].RoleName))
                    {
                        result = await userManager.AddToRoleAsync(user, model.Roles[i].RoleName);
                    }
                    else if (!model.Roles[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.Roles[i].RoleName);
                    }
                    else
                    {
                        continue;
                    }

                    if (result.Succeeded)
                    {
                        if (i < (model.Roles.Count - 1))
                            continue;
                        else
                            return RedirectToAction("ListUsers", "Administration");
                    }
                }
                return RedirectToAction("ListUsers", "Administration");
            }
            ViewBag.ErrorTitle = "User Not Found";
            return View("Error");

        }

        [HttpGet]
        public IActionResult CreateUser()
        {

            CreateUserViewModel model = new CreateUserViewModel();
            
            var roles = roleManager.Roles.ToList();
            var RoleList = new List<EditUserRoleViewModel>();
            for (int i = 0; i < roles.Count(); i++)
            {
                var vm = new EditUserRoleViewModel();
                vm.RoleName = roles[i].Name;
                vm.IsSelected = false;
                RoleList.Add(vm);
            }
            model.Roles = RoleList;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser();
                user.UserName = model.Username;
                user.Email = model.Email;
                user.SecurityQuestion1 = model.SecQuestion1;
                user.SecurityQuestion2 = model.SecQuestion2;
                user.NormalizedUserName = model.Username.ToUpper();
                user.NormalizedEmail = model.Email.ToUpper();

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var CreatedUser = await userManager.FindByNameAsync(model.Username);
                    for (int i = 0; i < model.Roles.Count; i++)
                    {
                        if (model.Roles[i].IsSelected)
                        {
                            await userManager.AddToRoleAsync(CreatedUser, model.Roles[i].RoleName);
                        }
                    }
                    return RedirectToAction("ListUsers", "Administration");
                }
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                DeleteUserViewModel model = new DeleteUserViewModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Id = user.Id
                };
                return View(model);
            }
            ViewBag.ErrorTitle = "The user that you want to delete cannot be found";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(DeleteUserViewModel model, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Administration");
                }
            }
            ViewBag.ErrorTitle = "The user that you want to delete cannot be found";
            return View("Error");
        }

        [HttpGet]
        public IActionResult ListCredits()
        {
            List<AppUser> users = userManager.Users.ToList();
            List<ListUsersViewModel> modelList = new List<ListUsersViewModel>(users.Count);

            foreach (var user in users)
            {
                ListUsersViewModel vm = new ListUsersViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Credits = user.Credits
                };
                modelList.Add(vm);
            }
            return View(modelList);
        }

        [HttpGet]
        public async Task<IActionResult> EditCredits(string id)
        {
            var model = new EditCreditViewMOdel();
            var user = await userManager.FindByIdAsync(id);

            model.User = user;
            model.Credits = user.Credits;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCredits(string id, EditCreditViewMOdel model)
        {
            var user = await userManager.FindByIdAsync(id);
            user.Credits = model.Credits;
            await userManager.UpdateAsync(user);
            return RedirectToAction("ListCredits");
        }
    }
}