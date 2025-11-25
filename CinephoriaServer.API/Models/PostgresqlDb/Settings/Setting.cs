using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Setting : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du paramètre.
        /// </summary>
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "La clé du paramètre ne peut pas dépasser 255 caractères.")]
        /// <summary>
        /// Clé unique du paramètre (ex: 'general.companyName', 'security.passwordMinLength').
        /// </summary>
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Valeur du paramètre stockée en texte.
        /// </summary>
        public string SettingValue { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "La catégorie ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Catégorie du paramètre ('general', 'notifications', 'security').
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Description du paramètre.
        /// </summary>
        public string? Description { get; set; }
    }
}