using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Repository
{
    public class ShowtimeConfig : IEntityTypeConfiguration<Showtime>
    {
        public void Configure(EntityTypeBuilder<Showtime> builder)
        {
            builder.HasKey(s => s.ShowtimeId);

            builder.Property(s => s.StartTime)
                   .IsRequired();

            builder.Property(s => s.EndTime)
                   .IsRequired();

            builder.Property(s => s.Quality)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.Price)
                   .IsRequired();

            builder.Property(s => s.AvailableSeats)
                   .IsRequired();

            // One-to-many relationship between Showtime and Reservation
            builder.HasMany(s => s.Reservations)
                   .WithOne(r => r.Showtime)
                   .HasForeignKey(r => r.ShowtimeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
