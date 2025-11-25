using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinephoriaServer.API.Repository
{
    public class UserFavoriteConfig : IEntityTypeConfiguration<UserFavorite>
    {
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            // Clé primaire
            builder.HasKey(uf => uf.UserFavoriteId);

            // Propriétés
            builder.Property(uf => uf.AppUserId)
                   .IsRequired();

            builder.Property(uf => uf.MovieId)
                   .IsRequired();

            builder.Property(uf => uf.AddedAt)
                   .IsRequired();

            // Relations
            builder.HasOne(uf => uf.AppUser)
                   .WithMany(u => u.UserFavorites)
                   .HasForeignKey(uf => uf.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uf => uf.Movie)
                   .WithMany()
                   .HasForeignKey(uf => uf.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index composite pour éviter les doublons
            builder.HasIndex(uf => new { uf.AppUserId, uf.MovieId })
                   .IsUnique();
        }
    }
}