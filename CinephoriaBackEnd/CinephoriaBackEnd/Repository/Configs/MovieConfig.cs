using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Repository
{
    public class MovieConfig : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.MovieId);

            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(m => m.PosterUrl)
                   .HasMaxLength(500);

            builder.Property(m => m.AgeLimit)
                   .IsRequired();

            builder.Property(m => m.Favorite)
                   .HasDefaultValue(false);

            builder.Property(m => m.RatingAverage)
                   .HasDefaultValue(0f);

            // One-to-many relationship between Movie and Showtime
            builder.HasMany(m => m.Showtimes)
                   .WithOne(s => s.Movie)
                   .HasForeignKey(s => s.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between Movie and Review
            builder.HasMany(m => m.Reviews)
                   .WithOne(r => r.Movie)
                   .HasForeignKey(r => r.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
