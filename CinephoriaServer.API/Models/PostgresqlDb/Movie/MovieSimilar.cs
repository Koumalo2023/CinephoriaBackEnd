using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    [Table("MovieSimilars")]
    public class MovieSimilar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de la relation de similarité.
        /// </summary>
        public int MovieSimilarId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant du film source.
        /// </summary>
        public int SourceMovieId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant du film similaire.
        /// </summary>
        public int SimilarMovieId { get; set; }

        /// <summary>
        /// Score de similarité entre les films (optionnel).
        /// </summary>
        public double? SimilarityScore { get; set; }

        /// <summary>
        /// Date de création de la relation de similarité.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Film source de la relation.
        /// </summary>
        [ForeignKey("SourceMovieId")]
        public virtual Movie SourceMovie { get; set; } = null!;

        /// <summary>
        /// Film similaire.
        /// </summary>
        [ForeignKey("SimilarMovieId")]
        public virtual Movie SimilarMovie { get; set; } = null!;
    }
}