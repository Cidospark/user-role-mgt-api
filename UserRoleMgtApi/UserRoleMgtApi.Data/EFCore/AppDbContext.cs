using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Data.EFCore
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
