using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class MovieRatingConfig : IEntityTypeConfiguration<MovieRating>
    {
        public void Configure(EntityTypeBuilder<MovieRating> builder)
        {
            // Clé primaire
            builder.HasKey(r => r.MovieRatingId);

            // Propriétés
            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(500)
                   .IsRequired(); // Peut être requis en fonction de vos règles métier

            builder.Property(r => r.IsValidated)
                   .HasDefaultValue(false);

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.Property(r => r.UpdatedAt)
                   .IsRequired();

            // Relation many-to-one avec Movie
            builder.HasOne(r => r.Movie)
                   .WithMany(m => m.MovieRatings)
                   .HasForeignKey(r => r.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation many-to-one avec AppUser
            builder.HasOne(r => r.AppUser)
                   .WithMany(u => u.MovieRatings)
                   .HasForeignKey(r => r.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
