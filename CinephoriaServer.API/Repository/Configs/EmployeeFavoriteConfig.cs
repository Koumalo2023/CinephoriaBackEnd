using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Repository
{
    public class EmployeeFavoriteConfig : IEntityTypeConfiguration<EmployeeFavorite>
    {
        public void Configure(EntityTypeBuilder<EmployeeFavorite> builder)
        {
            // Clé primaire
            builder.HasKey(ef => ef.EmployeeFavoriteId);

            // Propriétés
            builder.Property(ef => ef.Comment)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(ef => ef.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(ef => ef.CreatedAt)
                   .IsRequired();

            builder.Property(ef => ef.UpdatedAt)
                   .IsRequired();

            // Relation many-to-one avec Movie
            builder.HasOne(ef => ef.Movie)
                   .WithMany()
                   .HasForeignKey(ef => ef.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation many-to-one avec AppUser
            builder.HasOne(ef => ef.AppUser)
                   .WithMany(u => u.EmployeeFavorites)
                   .HasForeignKey(ef => ef.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index composite pour éviter les doublons
            builder.HasIndex(ef => new { ef.AppUserId, ef.MovieId })
                   .IsUnique();
        }
    }
}