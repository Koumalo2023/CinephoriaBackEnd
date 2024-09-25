using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Repository.Configs
{
    public class ReservationConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.ReservationId);

            builder.Property(r => r.SeatNumber)
                   .IsRequired();

            builder.Property(r => r.Price)
                   .IsRequired();

            builder.Property(r => r.QrCode)
                   .HasMaxLength(100);

            builder.Property(r => r.Status)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.Property(r => r.UpdatedAt)
                   .IsRequired();

            // Many-to-one relationship between Reservation and AppUser
            builder.HasOne(r => r.AppUser)
                   .WithMany(u => u.Reservations)
                   .HasForeignKey(r => r.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
