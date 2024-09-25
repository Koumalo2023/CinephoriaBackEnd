namespace CinephoriaBackEnd.Models
{
    public class ReviewDto
    {
        /// <summary>
        /// Identifiant unique de la critique.
        /// </summary>
        public int ReviewId { get; set; }

        /// <summary>
        /// Identifiant du film critiqué.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant laissé la critique.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Note attribuée au film.
        /// </summary>
        public float Rating { get; set; }

        /// <summary>
        /// Commentaire de la critique.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Indique si la critique est approuvée.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Date de création de la critique.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour de la critique.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

}
