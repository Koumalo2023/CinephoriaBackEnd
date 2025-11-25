using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto
{
    public class UpdateEmployeeDto
    {
        /// <summary>
        /// Identifiant unique de l'employé à mettre à jour.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Nouvelle adresse e-mail de l'employé.
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }

        /// <summary>
        /// Nouveau prénom de l'employé.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        /// <summary>
        /// Nouveau nom de famille de l'employé.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        /// <summary>
        /// Nouveau poste ou fonction de l'employé.
        /// </summary>
        [StringLength(100, ErrorMessage = "Le poste ne peut pas dépasser 100 caractères.")]
        public string Position { get; set; }


        [Required(ErrorMessage = "La date d'embauche est obligatoire.")]
        public DateTime? HiredDate { get; set; }


        /// <summary>
        /// Numero de téléphonce de l'utilisateur .
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Nouvelle URL de l'image de profil de l'employé.
        /// </summary>
        [Url(ErrorMessage = "L'URL de l'image de profil n'est pas valide.")]
        public string ProfilePictureUrl { get; set; }
    }
}
