﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class DeleteUserViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        
    }
}
