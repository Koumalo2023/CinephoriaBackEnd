using CinephoriaServer.API.Configurations.Extensions;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using CinephoriaServer.API.Repository.Configs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Data
{
    public class CinephoriaDbContext : IdentityDbContext<AppUser>
    {
        #region Common
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Theater> Theaters => Set<Theater>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<Showtime> Showtimes => Set<Showtime>();
        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<MovieRating> MovieRatings => Set<MovieRating>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<UserMovieHistory> UserMovieHistories => Set<UserMovieHistory>();
        public DbSet<EmployeeFavorite> EmployeeFavorites => Set<EmployeeFavorite>();
        public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<MovieSimilar> MovieSimilars => Set<MovieSimilar>();

        #endregion

        public CinephoriaDbContext(DbContextOptions<CinephoriaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new SeatConfig());
            modelBuilder.ApplyConfiguration(new MovieConfig());
            modelBuilder.ApplyConfiguration(new TheaterConfig());
            modelBuilder.ApplyConfiguration(new IncidentConfig());
            modelBuilder.ApplyConfiguration(new ShowtimeConfig());
            modelBuilder.ApplyConfiguration(new CinemaConfig());
            modelBuilder.ApplyConfiguration(new MovieRatingConfig());
            modelBuilder.ApplyConfiguration(new ReservationConfig());
            modelBuilder.ApplyConfiguration(new UserMovieHistoryConfig());
            modelBuilder.ApplyConfiguration(new EmployeeFavoriteConfig());
            modelBuilder.ApplyConfiguration(new UserFavoriteConfig());
            modelBuilder.ApplyConfiguration(new SettingConfig());
            modelBuilder.ApplyConfiguration(new MovieSimilarConfig());


            modelBuilder.Ignore<AdminDashboard>();

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (BaseEntity)entityEntry.Entity;
                var now = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                }

                entity.UpdatedAt = now;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
