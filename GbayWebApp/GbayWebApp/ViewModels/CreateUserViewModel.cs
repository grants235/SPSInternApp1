using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
        ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Security Questions 1: What is the name of your first pet")]
        public string SecQuestion1 { get; set; }

        [Required]
        [Display(Name = "Security Questions 2: What is your mothers maiden name")]
        public string SecQuestion2 { get; set; }

        public List<EditUserRoleViewModel> Roles { get; set; }
    }
}
