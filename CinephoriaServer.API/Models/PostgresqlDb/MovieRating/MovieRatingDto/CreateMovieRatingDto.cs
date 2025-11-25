namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CreateMovieRatingDto
    {
        /// <summary>
        /// Identifiant du film pour lequel l'avis est laissé.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur laissant l'avis.
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
    }
}
