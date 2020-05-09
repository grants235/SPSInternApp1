using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class ResetEmailViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Old Email")]
        public string OldEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }

    }
}
