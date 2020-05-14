using GbayWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class ListRolesIndivudalViewModel
    {
        public ListRolesIndivudalViewModel()
        {
            this.Users = new List<AppUser>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public List<AppUser> Users { get; set; }
    }
}
