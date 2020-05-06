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

namespace GbayWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.SecurityQuestion1 = model.SecurityQuestion1;
                user.SecurityQuestion2 = model.SecurityQuestion2;
                await userManager.UpdateAsync(user);
                return RedirectToAction("index", "home");
            }
            else
            {
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
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                TempData["UserEmail"] = model.Email;
                TempData["UserPassword"] = model.Password;

                if (result.Succeeded)
                {
                    await signInManager.SignOutAsync();
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