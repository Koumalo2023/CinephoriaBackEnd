using CinephoriaBackEnd.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Data
{
    public class CinephoriaDbContext : IdentityDbContext<AppUser>
    {

        public CinephoriaDbContext(DbContextOptions<CinephoriaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
