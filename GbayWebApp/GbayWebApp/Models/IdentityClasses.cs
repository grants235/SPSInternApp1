using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.Models
{
    public class IdentityClasses
    {
    }

    public class AppUser : IdentityUser<long>
    {
        public AppUser()
        {
            if (this.CartProducts == null)
            {
                this.CartProducts = new List<Product>();
            }
        }

        public string SecurityQuestion1 { get; set; }
        public string SecurityQuestion2 { get; set; }

        public List<Product> CartProducts { get; set; }
    }



    public class AppRole : IdentityRole<long>
    {
        public AppRole() : base()
        {

        }

        public AppRole(string roleName)
        {
            Name = roleName;
        }
    }
}
