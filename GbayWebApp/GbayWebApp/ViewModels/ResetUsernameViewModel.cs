using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class ResetUsernameViewModel
    {
        [Required]
        public string OldUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string NewUsername { get; set; }
    }
}
