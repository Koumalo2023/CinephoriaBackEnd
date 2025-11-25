using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class SeatConfig : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            // Clé primaire
            builder.HasKey(s => s.SeatId);

            // Propriétés
            builder.Property(s => s.SeatNumber)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(s => s.IsAccessible)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(s => s.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            // Relation avec Theater
            builder.HasOne(s => s.Theater)
                   .WithMany(t => t.Seats)
                   .HasForeignKey(s => s.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Reservation
            builder.HasMany(s => s.Reservations)
                   .WithMany(r => r.Seats)
                   .UsingEntity(j => j.ToTable("SeatReservations"));
        }
    }
}
