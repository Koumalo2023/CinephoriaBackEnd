using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class TheaterConfig : IEntityTypeConfiguration<Theater>
    {
        public void Configure(EntityTypeBuilder<Theater> builder)
        {
            // Clé primaire
            builder.HasKey(t => t.TheaterId);

            // Propriétés
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.SeatCount)
                   .IsRequired();

            builder.Property(t => t.IsOperational)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(t => t.ProjectionQuality)
                   .IsRequired();

            // Relation avec Cinema
            builder.HasOne(t => t.Cinema)
                   .WithMany(c => c.Theaters)
                   .HasForeignKey(t => t.CinemaId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Showtime
            builder.HasMany(t => t.Showtimes)
                   .WithOne(s => s.Theater)
                   .HasForeignKey(s => s.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Incident
            builder.HasMany(t => t.Incidents)
                   .WithOne(i => i.Theater)
                   .HasForeignKey(i => i.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Seat
            builder.HasMany(t => t.Seats)
                   .WithOne(s => s.Theater)
                   .HasForeignKey(s => s.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
