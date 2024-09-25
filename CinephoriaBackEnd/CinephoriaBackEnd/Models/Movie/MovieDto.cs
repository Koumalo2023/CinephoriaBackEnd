namespace CinephoriaBackEnd.Models
{
    public class MovieDto
    {
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description courte du film.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL de l'affiche du film.
        /// </summary>
        public string PosterUrl { get; set; }

        /// <summary>
        /// Limite d'âge pour le film.
        /// </summary>
        public int AgeLimit { get; set; }

        /// <summary>
        /// Indicateur si le film est un favori.
        /// </summary>
        public bool Favorite { get; set; }

        /// <summary>
        /// Note moyenne attribuée au film.
        /// </summary>
        public float RatingAverage { get; set; }

        /// <summary>
        /// Date de création du film.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour des informations du film.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

}
