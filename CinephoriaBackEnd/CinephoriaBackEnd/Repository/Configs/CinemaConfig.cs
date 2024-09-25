using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Repository
{
    public class CinemaConfig : IEntityTypeConfiguration<Cinema>
    {
        public void Configure(EntityTypeBuilder<Cinema> builder)
        {
            builder.HasKey(c => c.CinemaId);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.PhoneNumber)
                   .HasMaxLength(15);

            builder.Property(c => c.City)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Country)
                   .IsRequired()
                   .HasMaxLength(50);

            // One-to-many relationship between Cinema and Room
            builder.HasMany(c => c.Rooms)
                   .WithOne(r => r.Cinema)
                   .HasForeignKey(r => r.CinemaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
