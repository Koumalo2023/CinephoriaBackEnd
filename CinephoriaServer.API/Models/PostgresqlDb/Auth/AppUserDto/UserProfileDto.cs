namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UserProfileDto
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
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Date de mise-à-jour du compte.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Liste des réservations de l'utilisateur.
        /// </summary>
        public ICollection<ReservationDto> Reservations { get; set; } = new List<ReservationDto>();

        public string PhoneNumber { get; set; }

        /// <summary>
        /// Liste des avis laissés par l'utilisateur.
        /// </summary>
        public ICollection<MovieRatingDto> MovieRatings { get; set; } = new List<MovieRatingDto>();

        /// <summary>
        /// Rôle de l'utilisateur.
        /// </summary>
        public string Role { get; set; }

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
