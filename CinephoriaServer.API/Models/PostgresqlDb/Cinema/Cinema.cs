using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Cinema : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du cinéma.
        /// </summary>
        public int CinemaId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Le nom du cinéma ne peut pas dépasser 100 caractères.")]
        /// <summary>
        /// Nom du cinéma.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Adresse du cinéma.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        /// <summary>
        /// Numéro de téléphone du cinéma.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Ville où se trouve le cinéma.
        /// </summary>
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Le pays ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Pays (France, Belgique, etc.).
        /// </summary>
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(500, ErrorMessage = "Les horaires d'ouverture ne peuvent pas dépasser 500 caractères.")]
        /// <summary>
        /// Horaires d'ouverture du cinéma.
        /// </summary>
        public string OpeningHours { get; set; } = string.Empty;

        /// <summary>
        /// Liste des séances disponibles dans ce cinéma (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

        /// <summary>
        /// Liste des salles disponibles dans ce cinéma (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Theater> Theaters { get; set; } = new List<Theater>();
    }
}
