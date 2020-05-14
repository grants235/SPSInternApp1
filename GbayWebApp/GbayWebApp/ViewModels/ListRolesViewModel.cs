using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class ListRolesViewModel
    {
        public ListRolesViewModel()
        {
            this.Roles = new List<ListRolesIndivudalViewModel>();
        }
        public List<ListRolesIndivudalViewModel> Roles { get; set; }
    }
}
