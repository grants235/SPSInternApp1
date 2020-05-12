using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GbayWebApp.Data;
using GbayWebApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GbayWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "GbayWebApplication"));

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 3;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            AppRole adminRole = new AppRole
            {
                Name = "Administrators"
            };
            await roleManager.CreateAsync(adminRole);

            AppRole buyerRole = new AppRole
            {
                Name = "Buyers"
            };
            await roleManager.CreateAsync(buyerRole);

            AppRole sellerRole = new AppRole
            {
                Name = "Sellers"
            };
            await roleManager.CreateAsync(sellerRole);

            AppRole moderatorRole = new AppRole
            {
                Name = "Moderators"
            };
            await roleManager.CreateAsync(moderatorRole);

            var adminUser = new AppUser()
                {
                    UserName = "Administrator",
                    Email = "admin@admin.com",
                    SecurityQuestion1 = "a",
                    SecurityQuestion2 = "a",
                    EmailConfirmed = true
                };
            await userManager.CreateAsync(adminUser, "P@ssword1");
            var result = await userManager.AddToRoleAsync(adminUser, adminRole.Name);

            var buyerUser = new AppUser()
            {
                UserName = "Buyer",
                Email = "buyer@buyer.com",
                SecurityQuestion1 = "b",
                SecurityQuestion2 = "b",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(buyerUser, "P@ssword1");
            result = await userManager.AddToRoleAsync(buyerUser, buyerRole.Name);

            var sellerUser = new AppUser()
            {
                UserName = "Seller",
                Email = "seller@seller.com",
                SecurityQuestion1 = "s",
                SecurityQuestion2 = "s",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(sellerUser, "P@ssword1");
            result = await userManager.AddToRoleAsync(sellerUser, sellerRole.Name);

            var moderatorUser = new AppUser()
            {
                UserName = "Moderator",
                Email = "moderator@moderator.com",
                SecurityQuestion1 = "m",
                SecurityQuestion2 = "m",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(moderatorUser, "P@ssword1");
            result = await userManager.AddToRoleAsync(moderatorUser, moderatorRole.Name);

        }
    }
}

