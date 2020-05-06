using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class LoginSecurityQuestions
    {
        [Required]
        [Display(Name = "Security Questions 1: What is the name of your first pet")]
        public string SecurityQuestion1 { get; set; }

        [Required]
        [Display(Name = "Security Questions 2: What is your mothers maiden name")]
        public string SecurityQuestion2 { get; set; }
    }
}
