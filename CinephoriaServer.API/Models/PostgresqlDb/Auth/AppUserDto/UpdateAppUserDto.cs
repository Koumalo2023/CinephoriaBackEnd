using System.ComponentModel.DataAnnotations; 

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateAppUserDto
    {
        [Required(ErrorMessage = "L'identifiant de l'utilisateur est obligatoire.")]
        public string AppUserId { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom de famille est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 50 caractères.")]
        public string UserName { get; set; }
        
        public string ProfilePictureUrl { get; set; }

        public string PhoneNumber { get; set; }
    }
}
