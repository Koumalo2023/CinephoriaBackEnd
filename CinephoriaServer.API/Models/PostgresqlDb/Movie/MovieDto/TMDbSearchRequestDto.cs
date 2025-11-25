using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class TMDbSearchRequestDto
    {
        [Required(ErrorMessage = "Le terme de recherche est requis")]
        [StringLength(100, ErrorMessage = "Le terme de recherche ne peut pas dépasser 100 caractères")]
        public string Query { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Le numéro de page doit être compris entre 1 et 1000")]
        public int Page { get; set; } = 1;
    }
}