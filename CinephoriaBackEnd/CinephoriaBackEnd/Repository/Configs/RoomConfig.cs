using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Repository.Configs
{
    public class RoomConfig : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.RoomId);

            builder.Property(r => r.NumberOfSeats)
                   .IsRequired();

            builder.Property(r => r.ProjectionQuality)
                   .IsRequired()
                   .HasMaxLength(50);

            // One-to-many relationship between Room and Showtime
            builder.HasMany(r => r.Showtimes)
                   .WithOne(s => s.Room)
                   .HasForeignKey(s => s.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between Room and Incident
            builder.HasMany(r => r.Incidents)
                   .WithOne(i => i.Room)
                   .HasForeignKey(i => i.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
