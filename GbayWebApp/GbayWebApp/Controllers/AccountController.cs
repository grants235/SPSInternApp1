﻿using System;
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
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.Identity;

namespace GbayWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IConfiguration configuration;

        public AccountController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.configuration = configuration;
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
            TempData["CreateUsername"] = model.Username;

            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Username, Email = model.Email };

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
            string userName = TempData["CreateUsername"].ToString();
            string userPassword = TempData["CreateUserPassword"].ToString();
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByNameAsync(userName);

            if (user != null)
            {
                user.SecurityQuestion1 = model.SecurityQuestion1;
                user.SecurityQuestion2 = model.SecurityQuestion2;
                await userManager.UpdateAsync(user);

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

                ViewBag.ErrorTitle = "Registration Suscessful";
                ViewBag.ErrorMessage = "Befor you can login, please confirm your email by clicking on the confirmation link we have emailed you.";
                return View("Error");
            }
            
            else
            {
                ViewBag.ErrorMessage = "Error Registering please try again";
                ViewBag.ErrorTitle = "Error";
                return View("Error");
            }
            

        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = $"The user ID {userId} is not valid";
                return View("Error");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");

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
                var user = await userManager.FindByNameAsync(model.Username);
                if (user.Email != model.Email)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attepmt");
                    return View(model);
                }
                
                if (user != null && !user.EmailConfirmed && 
                                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var result = await userManager.CheckPasswordAsync(user, model.Password);
                TempData["Username"] = model.Username;
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
            string Username = TempData["Username"].ToString();
            string Password = TempData["UserPassword"].ToString();
            var user = await userManager.FindByNameAsync(Username);
            if ((user.SecurityQuestion1 == model.SecurityQuestion1) && (user.SecurityQuestion2 == model.SecurityQuestion2))
            {
                var result = await signInManager.PasswordSignInAsync(Username, Password, false, false);
                TempData["UserEmail"] = null;
                TempData["UserPassword"] = null;

                return RedirectToAction("index", "home");
            }
            else
            {
                TempData["Username"] = Username;
                TempData["UserPassword"] = Password;
                ModelState.AddModelError(string.Empty, "Invalid security question answers");
            }

            return View();
           
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                    SmtpClient client = new SmtpClient();
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate(configuration["EmailUsernameSecret"], configuration["EmailPasswordsecret"]);

                    MimeMessage message = new MimeMessage();
                    MailboxAddress from = new MailboxAddress("Grant Shanklin", "grantshanklintest@gmail.com");
                    message.From.Add(from);
                    MailboxAddress to = new MailboxAddress(user.UserName, user.Email);
                    message.To.Add(to);
                    message.Subject = "Reset Password";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = $"To reset your password, click on the link: {passwordResetLink}";
                    message.Body = bodyBuilder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();

                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult MyAccount()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetSecQuestions()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> ResetSecQuestions(ResetSecQuestionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                var CheckPassword = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (CheckPassword.Succeeded)
                {
                    user.SecurityQuestion1 = model.SecurityQuestion1;
                    user.SecurityQuestion2 = model.SecurityQuestion2;
                    await userManager.UpdateAsync(user);

                    return RedirectToAction("MyAccount");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Incorrect Password");
                    return View(model);
                }

            }
            return View(model);
        }
        
        
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogoutConfirm()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult ResetEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetEmail(ResetEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                var CheckPassword = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (CheckPassword.Succeeded)
                {
                    user.Email = model.NewEmail;
                    user.UserName = model.NewEmail;
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("MyAccount");
                }
            }
            return View(model);
        }
    }
}