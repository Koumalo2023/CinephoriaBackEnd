
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface IShowtimeService
    {
        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto);

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="updateShowtimeDto">Les données mises à jour de la séance.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> UpdateShowtimeAsync(UpdateShowtimeDto updateShowtimeDto);

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> DeleteShowtimeAsync(int showtimeId);

        /// <summary>
        /// Récupère la liste de toutes les séances.
        /// </summary>
        /// <returns>Une liste de séances sous forme de DTO.</returns>
        Task<List<ShowtimeDto>> GetAllShowtimesAsync();

        /// <summary>
        /// Récupère les détails d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Les détails de la séance sous forme de DTO.</returns>
        Task<ShowtimeDto> GetShowtimeDetailsAsync(int showtimeId);

    }
}
