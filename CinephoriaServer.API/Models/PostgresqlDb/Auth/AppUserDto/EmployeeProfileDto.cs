namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class EmployeeProfileDto
    {
        /// <summary>
        /// Identifiant unique de l'employé.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Prénom de l'employé.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nom de famille de l'employé.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Adresse e-mail de l'employé.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Date d'embauche de l'employé.
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise-à-jour du compte.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// URL de l'image de profil de l'employé.
        /// </summary>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// Numero de téléphonce de l'utilisateur .
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Liste des incidents gérés par l'employé.
        /// </summary>
        public ICollection<IncidentDto> ResolvedByIncidents { get; set; } = new List<IncidentDto>();
        
        /// <summary>
        /// Liste des incidents signalés par l'employé (applicable aux utilisateurs de type "Employee").
        /// </summary>
        public ICollection<IncidentDto> ReportedIncidents { get; set; } = new List<IncidentDto>();

        /// <summary>
        /// Liste des coups de cœur employé.
        /// </summary>
        public ICollection<EmployeeFavoriteDto> EmployeeFavorites { get; set; } = new List<EmployeeFavoriteDto>();

        /// <summary>
        /// Rôle de l'utilisateur.
        /// </summary>
        public string Role { get; set; }
    }
}
