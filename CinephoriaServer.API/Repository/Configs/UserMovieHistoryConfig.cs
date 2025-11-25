using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Repository
{
    public class UserMovieHistoryConfig : IEntityTypeConfiguration<UserMovieHistory>
    {
        public void Configure(EntityTypeBuilder<UserMovieHistory> builder)
        {
            // Clé primaire
            builder.HasKey(h => h.UserMovieHistoryId);

            // Propriétés
            builder.Property(h => h.LastViewedAt)
                   .IsRequired();

            builder.Property(h => h.ViewCount)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(h => h.CreatedAt)
                   .IsRequired();

            builder.Property(h => h.UpdatedAt)
                   .IsRequired();

            // Relation many-to-one avec Movie
            builder.HasOne(h => h.Movie)
                   .WithMany()
                   .HasForeignKey(h => h.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation many-to-one avec AppUser
            builder.HasOne(h => h.AppUser)
                   .WithMany(u => u.UserMovieHistories)
                   .HasForeignKey(h => h.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index composite pour éviter les doublons
            builder.HasIndex(h => new { h.AppUserId, h.MovieId })
                   .IsUnique();
        }
    }
}