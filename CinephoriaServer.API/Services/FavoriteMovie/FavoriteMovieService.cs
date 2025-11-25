using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using CinephoriaServer.API.Services.FavoriteMovie;

namespace CinephoriaServer.API.Services.FavoriteMovie
{
    public class FavoriteMovieService : IFavoriteMovieService
    {
        private readonly IUserRepository _userRepository;

        public FavoriteMovieService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Ajoute un film aux favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        public async Task AddMovieToFavoritesAsync(string userId, int movieId)
        {
            await _userRepository.AddMovieToFavoritesAsync(userId, movieId);
        }

        /// <summary>
        /// Supprime un film des favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        public async Task RemoveMovieFromFavoritesAsync(string userId, int movieId)
        {
            await _userRepository.RemoveMovieFromFavoritesAsync(userId, movieId);
        }

        /// <summary>
        /// Récupère la liste des films favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>La liste des films favoris.</returns>
        public async Task<List<Movie>> GetUserFavoriteMoviesAsync(string userId)
        {
            return await _userRepository.GetUserFavoriteMoviesAsync(userId);
        }

        /// <summary>
        /// Vérifie si un film est dans les favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est dans les favoris, sinon false.</returns>
        public async Task<bool> IsMovieInFavoritesAsync(string userId, int movieId)
        {
            return await _userRepository.IsMovieInFavoritesAsync(userId, movieId);
        }
    }
}