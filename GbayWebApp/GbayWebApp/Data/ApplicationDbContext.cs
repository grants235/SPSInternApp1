using GbayWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace GbayWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}