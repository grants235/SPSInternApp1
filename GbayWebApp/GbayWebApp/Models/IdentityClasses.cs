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
