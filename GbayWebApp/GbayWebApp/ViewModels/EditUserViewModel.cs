using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        [Display(Name = "Security Questions 1: What is the name of your first pet")]
        public string SecQuestion1 { get; set; }
        [Display(Name = "Security Questions 2: What is your mothers maiden name")]
        public string SecQuestion2 { get; set; }
    }
}
