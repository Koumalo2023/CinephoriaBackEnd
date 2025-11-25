using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class AppUser : IdentityUser
    {
        // --- Informations générales ---

        [Required]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Prénom de l'utilisateur.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Nom de famille de l'utilisateur.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de mise à jour du compte.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indique si l'utilisateur a approuvé les conditions d'utilisation.
        /// </summary>
        public bool HasApprovedTermsOfUse { get; set; } = false;

        // --- Propriétés héritées de EmployeeAccount ---

        /// <summary>
        /// Date d'embauche de l'employé.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public DateTime? HiredDate { get; set; }

        [StringLength(100, ErrorMessage = "Le poste ne peut pas dépasser 100 caractères.")]
        /// <summary>
        /// Poste ou fonction de l'employé.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public string? Position { get; set; }

        [Url(ErrorMessage = "L'URL de l'image de profil n'est pas valide.")]
        /// <summary>
        /// Image du profil des employés.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Liste des incidents signalés par l'employé.
        /// Applicable uniquement pour les employés.
        /// </summary>
        public ICollection<Incident> ReportedIncidents { get; set; } = new List<Incident>();


        /// <summary>
        /// Liste des incidents resolus par l'employé.
        /// Applicable uniquement pour les employés.
        /// </summary>
        public ICollection<Incident> ResolvedByIncidents { get; set; } = new List<Incident>();

        // --- Propriétés liées à ASP.NET Identity ---

        /// <summary>
        /// Rôle(s) de l'utilisateur : Admin, Employee, User.
        /// </summary>
        public UserRole Role { get; set; }

        // --- Autres propriétés liées aux utilisateurs ---

        /// <summary>
        /// Liste des réservations effectuées par l'utilisateur.
        /// Applicable aux utilisateurs normaux.
        /// </summary>
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        /// <summary>
        /// Liste des avis laissés par l'utilisateur.
        /// Applicable aux utilisateurs normaux.
        /// </summary>
        public ICollection<MovieRating> MovieRatings { get; set; } = new List<MovieRating>();

        /// <summary>
        /// Liste des films favoris de l'utilisateur.
        /// </summary>
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();

        /// <summary>
        /// Historique des films consultés par l'utilisateur.
        /// </summary>
        public ICollection<UserMovieHistory> UserMovieHistories { get; set; } = new List<UserMovieHistory>();

        /// <summary>
        /// Liste des coups de cœur employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public ICollection<EmployeeFavorite> EmployeeFavorites { get; set; } = new List<EmployeeFavorite>();
    }

}
