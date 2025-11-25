using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface IMovieRatingService
    {
        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="createMovieRatingDto">Les données de l'avis à soumettre.</param>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> SubmitMovieReviewAsync(CreateMovieRatingDto createMovieRatingDto, string AppUserId);

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> ValidateReviewAsync(int reviewId);

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> DeleteReviewAsync(int reviewId);

        /// <summary>
        /// Récupère la liste des avis associés à un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis sous forme de DTO.</returns>
        Task<List<MovieRatingDto>> GetMovieReviewsAsync(int movieId);

        /// <summary>
        /// Récupère les détails d'un avis spécifique.
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis.</param>
        /// <returns>Les détails de l'avis sous forme de DTO.</returns>
        Task<MovieRatingDto> GetReviewDetailsAsync(int reviewId);

        /// <summary>
        /// Met à jour un avis existant.
        /// </summary>
        /// <param name="updateMovieRatingDto">Les données mises à jour de l'avis.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> UpdateReviewAsync(UpdateMovieRatingDto updateMovieRatingDto);
    }
}
