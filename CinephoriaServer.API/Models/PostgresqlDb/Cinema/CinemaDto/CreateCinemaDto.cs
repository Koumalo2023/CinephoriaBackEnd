using System.ComponentModel.DataAnnotations; 

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    

    public class CreateCinemaDto
    {
        /// <summary>
        /// Nom du cinéma.
        /// </summary>
        [Required(ErrorMessage = "Le nom du cinéma est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom du cinéma ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        /// <summary>
        /// Adresse du cinéma.
        /// </summary>
        [Required(ErrorMessage = "L'adresse du cinéma est obligatoire.")]
        [MaxLength(200, ErrorMessage = "L'adresse du cinéma ne peut pas dépasser 200 caractères.")]
        public string Address { get; set; }

        /// <summary>
        /// Numéro de téléphone du cinéma.
        /// </summary>
        [Required(ErrorMessage = "Le numéro de téléphone est obligatoire.")]
        [Phone(ErrorMessage = "Le format du numéro de téléphone est invalide.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Ville où se trouve le cinéma.
        /// </summary>
        [Required(ErrorMessage = "La ville est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom de la ville ne peut pas dépasser 100 caractères.")]
        public string City { get; set; }

        /// <summary>
        /// Pays où se trouve le cinéma.
        /// </summary>
        [Required(ErrorMessage = "Le pays est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom du pays ne peut pas dépasser 100 caractères.")]
        public string Country { get; set; }

        /// <summary>
        /// Horaires d'ouverture du cinéma.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Les horaires d'ouverture ne peuvent pas dépasser 500 caractères.")]
        public string OpeningHours { get; set; }
    }
}
