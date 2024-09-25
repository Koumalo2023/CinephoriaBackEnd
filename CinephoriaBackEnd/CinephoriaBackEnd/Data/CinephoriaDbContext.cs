using Amazon.Util.Internal;
using CinephoriaBackEnd.Models;
using CinephoriaBackEnd.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Data
{
    public class CinephoriaDbContext : IdentityDbContext<AppUser>
    {
        #region Common 
        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Room> Rooms  => Set<Room>();
        public DbSet<Showtime> Showtimes => Set<Showtime>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Review> Reviews => Set<Review>();

        #endregion

        public CinephoriaDbContext(DbContextOptions<CinephoriaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CinemaConfig());
            modelBuilder.ApplyConfiguration(new MovieConfig());
            modelBuilder.ApplyConfiguration(new RoomConfig());
            modelBuilder.ApplyConfiguration(new ShowtimeConfig());
            modelBuilder.ApplyConfiguration(new ReviewConfig());
            modelBuilder.ApplyConfiguration(new ReservationConfig());
        }
    }
}
