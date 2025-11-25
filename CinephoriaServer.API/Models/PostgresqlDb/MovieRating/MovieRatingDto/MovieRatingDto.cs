
namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieRatingDto
    {
        /// <summary>
        /// Identifiant unique de l'avis.
        /// </summary>
        public int MovieRatingId { get; set; }

        /// <summary>
        /// Identifiant du film pour lequel l'avis a été laissé.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant laissé l'avis.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Note donnée par l'utilisateur (sur 5).
        /// </summary>
        public float Rating { get; set; }

        /// <summary>
        /// Description laissée par l'utilisateur.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Indique si l'avis a été approuvé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }
    }
}
