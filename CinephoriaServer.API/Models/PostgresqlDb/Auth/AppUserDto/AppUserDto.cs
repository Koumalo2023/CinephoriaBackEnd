using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class AppUserDto
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Prénom de l'utilisateur.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nom de famille de l'utilisateur.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Adresse e-mail de l'utilisateur.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nom d'utilisateur (username).
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Nom d'utilisateur (username).
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Numero de téléphonce de l'utilisateur .
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise à jour du compte.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Indique si l'utilisateur a approuvé les conditions d'utilisation.
        /// </summary>
        public bool HasApprovedTermsOfUse { get; set; }

        /// <summary>
        /// Date d'embauche de l'employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// URL de l'image de profil de l'employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// Liste des incidents signalés par l'employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public ICollection<IncidentDto> ReportedIncidents { get; set; } = new List<IncidentDto>();

        /// <summary>
        /// Liste des incidents resolus par l'employé.
        /// Applicable uniquement pour les employés.
        /// </summary>
        public ICollection<Incident> ResolvedByIncidents { get; set; } = new List<Incident>();

        /// <summary>
        /// Rôle(s) de l'utilisateur : ADMIN, EMPLOYEE, USER.
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Liste des réservations effectuées par l'utilisateur (applicable aux utilisateurs normaux).
        /// </summary>
        public ICollection<ReservationDto> Reservations { get; set; } = new List<ReservationDto>();

        /// <summary>
        /// Liste des avis laissés par l'utilisateur (applicable aux utilisateurs normaux).
        /// </summary>
        public ICollection<MovieRatingDto> MovieRatings { get; set; } = new List<MovieRatingDto>();

        /// <summary>
        /// Liste des films favoris de l'utilisateur.
        /// </summary>
        public ICollection<Movie> FavoriteMovies { get; set; } = new List<Movie>();

        /// <summary>
        /// Historique des films consultés par l'utilisateur.
        /// </summary>
        public ICollection<UserMovieHistoryDto> UserMovieHistories { get; set; } = new List<UserMovieHistoryDto>();

        /// <summary>
        /// Liste des coups de cœur employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public ICollection<EmployeeFavoriteDto> EmployeeFavorites { get; set; } = new List<EmployeeFavoriteDto>();
    }
}
