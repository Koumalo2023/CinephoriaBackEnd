using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Repository
{
    public class ShowtimeConfig : IEntityTypeConfiguration<Showtime>
    {
        public void Configure(EntityTypeBuilder<Showtime> builder)
        {
            // Clé primaire
            builder.HasKey(s => s.ShowtimeId);

            // Propriétés
            builder.Property(s => s.StartTime)
                   .IsRequired();

            builder.Property(s => s.EndTime)
                   .IsRequired();

            builder.Property(s => s.Quality)
                   .IsRequired();

            builder.Property(s => s.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18, 2)");

            builder.Property(s => s.Status)
                   .IsRequired()
                   .HasDefaultValue(ShowtimeStatus.Upcoming);

            builder.Property(s => s.ActualStartTime)
                   .IsRequired(false);

            builder.Property(s => s.ActualEndTime)
                   .IsRequired(false);

            builder.Property(s => s.OccupancyRate)
                   .IsRequired()
                   .HasDefaultValue(0);

            // Relation avec Movie
            builder.HasOne(s => s.Movie)
                   .WithMany(m => m.Showtimes)
                   .HasForeignKey(s => s.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Theater
            builder.HasOne(s => s.Theater)
                   .WithMany(t => t.Showtimes)
                   .HasForeignKey(s => s.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Reservation
            builder.HasMany(s => s.Reservations)
                   .WithOne(r => r.Showtime)
                   .HasForeignKey(r => r.ShowtimeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
