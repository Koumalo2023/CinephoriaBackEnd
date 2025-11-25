using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface ITheaterService
    {
        /// <summary>
        /// Récupère la liste des salles de cinéma associées à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de salles sous forme de DTO.</returns>
        Task<List<TheaterDto>> GetTheatersByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère la liste de tous les théâtres.
        /// </summary>
        /// <returns>Une liste de tous les théâtres sous forme de DTO.</returns>
        Task<List<TheaterDto>> GetAllTheatersAsync();

        /// <summary>
        /// Récupère une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>La salle correspondante sous forme de DTO.</returns>
        Task<TheaterDto> GetTheaterByIdAsync(int theaterId);

        /// <summary>
        /// Crée une nouvelle salle de cinéma.
        /// </summary>
        /// <param name="createTheaterDto">Les données de la salle à créer.</param>
        /// <returns>La salle créée sous forme de DTO.</returns>
        Task<string> CreateTheaterAsync(CreateTheaterDto createTheaterDto);

        /// <summary>
        /// Met à jour les informations d'une salle de cinéma existante.
        /// </summary>
        /// <param name="updateTheaterDto">Les données de la salle à mettre à jour.</param>
        /// <returns>La salle mise à jour sous forme de DTO.</returns>
        Task<string> UpdateTheaterAsync(UpdateTheaterDto updateTheaterDto);

        /// <summary>
        /// Supprime une salle de cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        Task<bool> DeleteTheaterAsync(int theaterId);

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        Task<List<IncidentDto>> GetTheaterIncidentsAsync(int theaterId);
    }
}
