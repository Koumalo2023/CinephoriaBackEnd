using CinephoriaBackEnd.Data;
using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore;


namespace CinephoriaBackEnd.Repository
{
    /// <summary>
    /// Interface pour gérer les opérations liées aux films
    /// </summary>
    public interface IMovieRepository
    {
        /// <summary>
        /// Récupérer la liste de tous les films disponibles
        /// </summary>
        /// <returns>Liste de films</returns>
        Task<List<Movie>> GetAllMovies(); // GET /api/films

        /// <summary>
        /// Ajouter un nouveau film
        /// </summary>
        /// <param name="movie">Les détails du film à ajouter</param>
        /// <returns>Le film créé</returns>
        Task<Movie> AddMovie(Movie movie); // POST /api/films

        /// <summary>
        /// Récupérer les détails d'un film par son ID
        /// </summary>
        /// <param name="id">L'ID du film</param>
        /// <returns>Les détails du film</returns>
        Task<Movie> GetMovieById(int id); // GET /api/films/{id}

        /// <summary>
        /// Mettre à jour les informations d'un film
        /// </summary>
        /// <param name="movie">Les détails mis à jour du film</param>
        Task UpdateMovie(Movie movie); // PUT /api/films/{id}

        /// <summary>
        /// Supprimer un film par son ID
        /// </summary>
        /// <param name="id">L'ID du film</param>
        Task DeleteMovie(int id); // DELETE /api/films/{id}
    }

    public class MovieRepository : IMovieRepository
    {
        private readonly CinephoriaDbContext _context;

        public MovieRepository(CinephoriaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie> GetMovieById(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task UpdateMovie(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
        }
    }

}
