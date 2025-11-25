using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface ICinemaService
    {
        /// <summary>
        /// Récupère la liste de tous les cinémas.
        /// </summary>
        /// <returns>Une liste de cinémas sous forme de DTO.</returns>
        Task<List<CinemaDto>> GetAllCinemasAsync();

        /// <summary>
        /// Récupère un cinéma par son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Le cinéma correspondant sous forme de DTO.</returns>
        Task<CinemaDto> GetCinemaByIdAsync(int cinemaId);

        /// <summary>
        /// Crée un nouveau cinéma.
        /// </summary>
        /// <param name="createCinemaDto">Les données du cinéma à créer.</param>
        /// <returns>Le cinéma créé sous forme de DTO.</returns>
        Task<string> CreateCinemaAsync(CreateCinemaDto createCinemaDto);

        /// <summary>
        /// Met à jour les informations d'un cinéma existant.
        /// </summary>
        /// <param name="updateCinemaDto">Les données du cinéma à mettre à jour.</param>
        /// <returns>Le cinéma mis à jour sous forme de DTO.</returns>
        Task<string> UpdateCinemaAsync(UpdateCinemaDto updateCinemaDto);

        /// <summary>
        /// Supprime un cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task<string> DeleteCinemaAsync(int cinemaId);
    }

}
