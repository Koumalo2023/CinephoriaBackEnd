using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Repository
{
    public class CinemaConfig : IEntityTypeConfiguration<Cinema>
    {
        public void Configure(EntityTypeBuilder<Cinema> builder)
        {
            // Clé primaire
            builder.HasKey(c => c.CinemaId);

            // Propriétés
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Address)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(c => c.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(c => c.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.OpeningHours)
                   .IsRequired()
                   .HasMaxLength(500);

            // Relation un-à-plusieurs avec Showtime
            builder.HasMany(c => c.Showtimes)
                   .WithOne(s => s.Cinema)
                   .HasForeignKey(s => s.CinemaId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs avec Theater
            builder.HasMany(c => c.Theaters)
                   .WithOne(t => t.Cinema)
                   .HasForeignKey(t => t.CinemaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
