using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;
using System.Collections.Generic;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateMovieDto
    {
        [Required(ErrorMessage = "L'identifiant du film est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant du film doit être un nombre positif.")]
        /// <summary>
        /// Identifiant unique du film à mettre à jour.
        /// </summary>
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Le titre du film est requis.")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Nouveau titre du film.
        /// </summary>
        public string Title { get; set; }

        [Required(ErrorMessage = "La description du film est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Nouvelle description du film.
        /// </summary>
        public string Description { get; set; }

        [Required(ErrorMessage = "Le genre du film est requis.")]
        [StringLength(50, ErrorMessage = "Le genre ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Nouveau genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        [Required(ErrorMessage = "La durée du film est requise.")]
        [Range(1, int.MaxValue, ErrorMessage = "La durée doit être un nombre positif.")]
        /// <summary>
        /// Nouvelle durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        [Required(ErrorMessage = "Le réalisateur du film est requis.")]
        /// <summary>
        /// Nouveau réalisateur du film.
        /// </summary>
        public List<string> Director { get; set; } = new List<string>();

        [Required(ErrorMessage = "La date de sortie du film est requise.")]
        /// <summary>
        /// Nouvelle date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "L'âge minimum recommandé est requis.")]
        /// <summary>
        /// Nouvel âge minimum recommandé pour visionner ce film.
        /// </summary>
        public MinimumAge MinimumAge { get; set; }

        /// <summary>
        /// Indique si le film est un "coup de cœur" de l'équipe du cinéma.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Liste des images associées au film.
        /// </summary>
        public string PosterUrls { get; set; }

        /// <summary>
        /// URL de la bande-annonce du film.
        /// </summary>
        [Url(ErrorMessage = "L'URL de la bande-annonce n'est pas valide.")]
        public string? BandeAnnonce { get; set; }

        [Required(ErrorMessage = "La liste des acteurs est requise.")]
        /// <summary>
        /// Nouvelle liste des acteurs du film.
        /// </summary>
        public List<string> Actors { get; set; } = new List<string>();
    }
}
