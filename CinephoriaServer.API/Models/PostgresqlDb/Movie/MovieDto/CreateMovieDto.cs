using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;
using System.Collections.Generic;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Le titre du film est requis.")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; }

        [Required(ErrorMessage = "La description du film est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; }

        [Required(ErrorMessage = "Le genre du film est requis.")]
        /// <summary>
        /// Genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        [Required(ErrorMessage = "La durée du film est requise.")]
        /// <summary>
        /// Durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        [Required(ErrorMessage = "Le réalisateur du film est requis.")]
        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public List<string> Director { get; set; } = new List<string>();

        [Required(ErrorMessage = "La date de sortie du film est requise.")]
        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "L'âge minimum recommandé est requis.")]
        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public MinimumAge MinimumAge { get; set; }

        /// <summary>
        /// Liste des images associées au film.
        /// </summary>
        public string? PosterUrls { get; set; }

        /// <summary>
        /// URL de la bande-annonce du film.
        /// </summary>
        [Url(ErrorMessage = "L'URL de la bande-annonce n'est pas valide.")]
        public string? BandeAnnonce { get; set; }

        [Required(ErrorMessage = "La liste des acteurs est requise.")]
        /// <summary>
        /// Liste des acteurs principaux du film.
        /// </summary>
        public List<string> Actors { get; set; } = new List<string>();
    }

}
