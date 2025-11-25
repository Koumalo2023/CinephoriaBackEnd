using CinephoriaServer.API.Configurations.Extensions;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateTheaterDto 
    {
        [Required(ErrorMessage = "L'identifiant de la salle est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de la salle doit être un nombre positif.")]
        /// <summary>
        /// Identifiant unique de la salle à mettre à jour.
        /// </summary>
        public int TheaterId { get; set; }

        [Required(ErrorMessage = "Le nom de la salle est requis.")]
        [StringLength(50, ErrorMessage = "Le nom de la salle ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Nouveau nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; }

        [Required(ErrorMessage = "La capacité de la salle est requise.")]
        [Range(1, int.MaxValue, ErrorMessage = "La capacité de la salle doit être un nombre positif.")]
        /// <summary>
        /// Nouvelle capacité maximale de la salle (nombre de sièges).
        /// </summary>
        public int SeatCount { get; set; }

        [Required(ErrorMessage = "L'identifiant du cinéma est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant du cinéma doit être un nombre positif.")]
        /// <summary>
        /// Identifiant du cinéma auquel appartient la salle.
        /// </summary>
        public int CinemaId { get; set; }

        [Required(ErrorMessage = "La qualité de projection est requise.")]
        /// <summary>
        /// Nouvelle qualité de projection disponible dans la salle (ex : "4DX", "3D", "4K").
        /// </summary>
        public ProjectionQuality ProjectionQuality { get; set; }


    }
}
