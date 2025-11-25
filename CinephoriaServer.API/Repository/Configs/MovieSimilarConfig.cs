using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinephoriaServer.API.Repository.Configs
{
    public class MovieSimilarConfig : IEntityTypeConfiguration<MovieSimilar>
    {
        public void Configure(EntityTypeBuilder<MovieSimilar> builder)
        {
            // Clé primaire
            builder.HasKey(ms => ms.MovieSimilarId);

            // Configuration des propriétés
            builder.Property(ms => ms.SourceMovieId)
                   .IsRequired();

            builder.Property(ms => ms.SimilarMovieId)
                   .IsRequired();

            builder.Property(ms => ms.SimilarityScore)
                   .IsRequired(false);

            builder.Property(ms => ms.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("NOW()");

            // Configuration des relations
            // Relation avec le film source
            builder.HasOne(ms => ms.SourceMovie)
                   .WithMany(m => m.SimilarMoviesAsSource)
                   .HasForeignKey(ms => ms.SourceMovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec le film similaire
            builder.HasOne(ms => ms.SimilarMovie)
                   .WithMany(m => m.SimilarMoviesAsTarget)
                   .HasForeignKey(ms => ms.SimilarMovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Contrainte pour éviter les doublons (même paire de films)
            builder.HasIndex(ms => new { ms.SourceMovieId, ms.SimilarMovieId })
                   .IsUnique();

            // Nom de la table
            builder.ToTable("MovieSimilars");
        }
    }
}