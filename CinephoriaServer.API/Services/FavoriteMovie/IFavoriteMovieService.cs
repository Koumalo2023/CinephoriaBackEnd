using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services.FavoriteMovie
{
    public interface IFavoriteMovieService
    {
        /// <summary>
        /// Ajoute un film aux favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        Task AddMovieToFavoritesAsync(string userId, int movieId);

        /// <summary>
        /// Supprime un film des favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        Task RemoveMovieFromFavoritesAsync(string userId, int movieId);

        /// <summary>
        /// Récupère la liste des films favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>La liste des films favoris.</returns>
        Task<List<Movie>> GetUserFavoriteMoviesAsync(string userId);

        /// <summary>
        /// Vérifie si un film est dans les favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est dans les favoris, sinon false.</returns>
        Task<bool> IsMovieInFavoritesAsync(string userId, int movieId);
    }
}