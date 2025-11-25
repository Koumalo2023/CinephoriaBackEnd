using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CreateEmployeeDto
    {
        /// <summary>
        /// Adresse e-mail de l'employé.
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }

        /// <summary>
        /// Mot de passe de l'employé.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Rôle(s) de l'utilisateur : Admin, Employee, User.
        /// </summary>
        [Required(ErrorMessage = "Le rôle est obligatoire.")]
        public UserRole Role { get; set; } = UserRole.Employee;

        /// <summary>
        /// Prénom de l'employé.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        /// <summary>
        /// Nom de famille de l'employé.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé.
        /// </summary>
        [StringLength(100, ErrorMessage = "Le poste ne peut pas dépasser 100 caractères.")]
        public string Position { get; set; }

        
        
        [Required(ErrorMessage = "La date d'embauche est obligatoire.")]
        public DateTime? HiredDate { get; set; }

        [Required(ErrorMessage = "Le numero de téléphone est obligatoire.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// URL de l'image de profil de l'employé.
        /// </summary>
        public string ProfilePictureUrl { get; set; }

    }
}
