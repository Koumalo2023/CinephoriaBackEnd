using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public interface IMovieRatingRepository : IReadRepository<MovieRating>, IWriteRepository<MovieRating>
    {
        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="movieRating">L'avis à soumettre.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task SubmitMovieReviewAsync(MovieRating movieRating);

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ValidateReviewAsync(int reviewId);

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteReviewAsync(int reviewId);

        /// <summary>
        /// Récupère la liste des avis associés à un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis.</returns>
        Task<List<MovieRating>> GetMovieReviewsAsync(int movieId);
    }

    public class MovieRatingRepository : EFRepository<MovieRating>, IMovieRatingRepository
    {
        private readonly DbContext _context;

        public MovieRatingRepository(DbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="movieRating">L'avis à soumettre.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task SubmitMovieReviewAsync(MovieRating movieRating)
        {
            movieRating.CreatedAt = DateTime.UtcNow;
            movieRating.UpdatedAt = DateTime.UtcNow;
            movieRating.IsValidated = false;

            _context.Set<MovieRating>().Add(movieRating);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task ValidateReviewAsync(int reviewId)
        {
            var review = await _context.Set<MovieRating>().FindAsync(reviewId);
            if (review == null)
            {
                throw new ArgumentException("Avis non trouvé.");
            }

            review.IsValidated = true;
            review.UpdatedAt = DateTime.UtcNow;

            _context.Set<MovieRating>().Update(review);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Set<MovieRating>().FindAsync(reviewId);
            if (review == null)
            {
                throw new ArgumentException("Avis non trouvé.");
            }

            _context.Set<MovieRating>().Remove(review);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère la liste des avis associés à un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis.</returns>
        public async Task<List<MovieRating>> GetMovieReviewsAsync(int movieId)
        {
            return await _context.Set<MovieRating>()
                .Include(mr => mr.Movie) 
                .Include(mr => mr.AppUser)
                .Where(mr => mr.MovieId == movieId)
                .ToListAsync();
        }


    }
}
