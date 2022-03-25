using AuthServer.Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    public class AppDbContext: IdentityDbContext<UserApp, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) //options işleminin "AppDbContext" üzerinde yapılacağını belirtiyoruz,
            //ismini ise "options" olarak veriyoruz. ":base(options)" ile base constructor'a yolluyoruz (identitydbcontext'in miras alındığı constructor'a)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken>  UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
