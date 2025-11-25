using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class FilterMoviesRequestDto
    {
        /// <summary>
        /// Identifiant du cinéma (optionnel).
        /// </summary>
        public int? CinemaId { get; set; }

        /// <summary>
        /// Genre du film (optionnel).
        /// </summary>
        public MovieGenre? Genre { get; set; }

        /// <summary>
        /// Date de projection (optionnelle).
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Année de sortie du film (optionnelle).
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Réalisateur du film (optionnel).
        /// </summary>
        [StringLength(100, ErrorMessage = "Le nom du réalisateur ne peut pas dépasser 100 caractères.")]
        public string? Director { get; set; }

        /// <summary>
        /// Acteur du film (optionnel).
        /// </summary>
        [StringLength(100, ErrorMessage = "Le nom de l'acteur ne peut pas dépasser 100 caractères.")]
        public string? Actor { get; set; }

        /// <summary>
        /// Classification par âge (optionnelle).
        /// </summary>
        public MinimumAge? MinimumAge { get; set; }
    }
}