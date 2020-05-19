using GbayWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;

namespace GbayWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<BuyCreditOption> BuyCreditOptions { get; set; }
    }

    public class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Products.Any())
                {
                    return;
                }

                context.Products.AddRange(

                    new Product
                    {
                        Id = 1,
                        ProductName = "Test Product",
                        Description = "This is a great test product to test ths web application",
                        Price = 19.99m,
                        Seller = "Administrator",
                        ImgUrl = "https://www.moltonbrown.sa/wp-content/uploads/sites/4/2018/06/test-product-not-for-sale-1.jpg"
                    },
                    new Product
                    {
                        Id = 2,
                        ProductName = "Test Product 2",
                        Description = "This is a great test product to test ths web application",
                        Price = 5.99m,
                        Seller = "Seller",
                        ImgUrl = "https://cdn.shopify.com/s/files/1/0532/2477/products/test-product.jpg?v=1432753385"
                    },
                    new Product
                    {
                        Id = 3,
                        ProductName = "Test Product 3",
                        Description = "This is a great test product to test ths web application",
                        Price = 12.50m,
                        Seller = "Administrator",
                        ImgUrl = "https://beta.pedallion.com/image/product/ORIGINAL/620348_test.gif"
                    },
                    new Product
                    {
                        Id = 4,
                        ProductName = "Test Product 4",
                        Description = "This is a great test product to test ths web application",
                        Price = 99.99m,
                        Seller = "Grant",
                        ImgUrl = "https://pfwo.com/image/cache/catalog/porto/index18/tstprod-500x500.jpg"
                    });

                context.BuyCreditOptions.AddRange(

                    new BuyCreditOption
                    {
                        Id = 1,
                        NumberOfCredits = 20,
                        Price = 15
                    },
                    new BuyCreditOption
                    {
                        Id = 2,
                        NumberOfCredits = 50,
                        Price = 40
                    });

                context.SaveChanges();

            }
        }
    }
}