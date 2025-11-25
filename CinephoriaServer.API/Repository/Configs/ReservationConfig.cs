using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class ReservationConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            // Clé primaire
            builder.HasKey(r => r.ReservationId);

            // Propriétés
            builder.Property(r => r.AppUserId)
                   .IsRequired();

            builder.Property(r => r.ShowtimeId)
                   .IsRequired();

            builder.Property(r => r.NumberOfSeats)
                   .IsRequired();

            builder.Property(r => r.TotalPrice)
                   .IsRequired();

            builder.Property(r => r.QrCode)
                   .IsRequired();

            builder.Property(r => r.Status)
                   .IsRequired();

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.Property(r => r.UpdatedAt)
                   .IsRequired();

            // Relations
            builder.HasOne(r => r.AppUser)
                   .WithMany(u => u.Reservations)
                   .HasForeignKey(r => r.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Showtime)
                   .WithMany(s => s.Reservations)
                   .HasForeignKey(r => r.ShowtimeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
