using System.Threading.Tasks;
using GbayWebApp.Models;
using GbayWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using GbayWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GbayWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;

        public ApplicationDbContext _context { get; }

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.Credits = user.Credits;
            }
            return View(await _context.Products.ToListAsync());
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Product(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        
    }
}
