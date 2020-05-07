using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GbayWebApp.Models;
using GbayWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace GbayWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            TempData["CreateUserPassword"] = model.Password;
            TempData["CreateUserEmail"] = model.Email;

            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("RegisterSecQuestions");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterSecQuestions()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> RegisterSecQuestions(RegisterSecQuestions model)
        {
            string userEmail = TempData["CreateUserEmail"].ToString();
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                user.SecurityQuestion1 = model.SecurityQuestion1;
                user.SecurityQuestion2 = model.SecurityQuestion2;
                await userManager.UpdateAsync(user);

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                       new { userId = user.Id, token = token }, Request.Scheme);

                logger.Log(LogLevel.Warning, confirmationLink);

                ViewBag.ErrorTitle = "Registration Suscessful";
                ViewBag.ErrorMessage = "Before you can login, please confirm your " +
                                       "email, by clicking on the confirmation link we have emailed you";
                return View("Error");
            }
            
            else
            {
                ViewBag.ErrorMessage = "Error Registering please try again";
                ViewBag.ErrorTitle = "Error";
                return View("Error");
            }
            

        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId ==null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                ViewBag.ErrorTitle = "Error";
                return View("Error");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }
            else
            {
                ViewBag.ErrorTitle = "Email Confirmation Error";
                ViewBag.ErrorMessage = "Please try to confirm your email again";

                return View("Error");
            }

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && 
                                      (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var result = await userManager.CheckPasswordAsync(user, model.Password);
                TempData["UserEmail"] = model.Email;
                TempData["UserPassword"] = model.Password;

                if (result == true)
                {
                    return RedirectToAction("LoginSecQuestions");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attepmt");
                
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult LoginSecQuestions()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginSecQuestions(LoginSecurityQuestions model)
        {
            string Email = TempData["UserEmail"].ToString();
            string Password = TempData["UserPassword"].ToString();
            var user = await userManager.FindByEmailAsync(Email);
            if ((user.SecurityQuestion1 == model.SecurityQuestion1) && (user.SecurityQuestion2 == model.SecurityQuestion2))
            {
                var result = await signInManager.PasswordSignInAsync(Email, Password, false, false);
                TempData["UserEmail"] = null;
                TempData["UserPassword"] = null;
                return RedirectToAction("index", "home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid security question answers");
            }

            return View();
           
        }

    }
}