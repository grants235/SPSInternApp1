using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GbayWebApp.Data;
using GbayWebApp.Models;
using GbayWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GbayWebApp.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> userManager;

        public ShoppingCartController(ShoppingCart shoppingCart, ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _shoppingCart = shoppingCart;
            _context = context;
            this.userManager = userManager;
        }

       
        public async Task<ViewResult> Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var model = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Credits = user.Credits;
            return View(model);
        }

        public RedirectToActionResult  AddToShoppingCart(int id)
        {
            var selectedProduct = _context.Products
                    .FirstOrDefault(m => m.Id == id);

            if (selectedProduct != null)
            {
                _shoppingCart.AddToCart(selectedProduct, 1);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int id)
        {
            var selectedProduct = _context.Products
                    .FirstOrDefault(m => m.Id == id);
            if (selectedProduct != null)
            {
                _shoppingCart.RemoveFromCart(selectedProduct);
            }
            return RedirectToAction("Index");
        }
    }
}