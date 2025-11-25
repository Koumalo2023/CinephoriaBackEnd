using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class TMDbImportRequestDto
    {
        [Required(ErrorMessage = "L'identifiant TMDb est requis")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant TMDb doit être un nombre positif")]
        public int TmdbId { get; set; }
    }
}