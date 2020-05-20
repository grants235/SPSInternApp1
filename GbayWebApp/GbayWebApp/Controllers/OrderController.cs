using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GbayWebApp.Models;
using GbayWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GbayWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ShoppingCart shoppingCart;

        public OrderController(UserManager<AppUser> userManager, ShoppingCart shoppingCart)
        {
            this.userManager = userManager;
            this.shoppingCart = shoppingCart;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var items = shoppingCart.GetShoppingCartItems();
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            foreach (var item in items)
            {
                var sellerUser = await userManager.FindByNameAsync(item.Product.Seller);
                user.Credits -= item.Product.Price;
                sellerUser.Credits += item.Product.Price;
                await userManager.UpdateAsync(sellerUser);
            }
            await userManager.UpdateAsync(user);
            shoppingCart.ClearCart();
            return RedirectToAction("Index", "Home");
        }
    }
}