using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public interface IIncidentService
    {
        /// <summary>
        /// Signale un nouvel incident dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="description">La description de l'incident.</param>
        /// <param name="reportedBy">L'identifiant de l'employé ayant signalé l'incident.</param>
        /// <param name="imageUrls">Liste des URLs des images associées à l'incident.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        Task<string> ReportIncidentAsync(CreateIncidentDto createIncidentDto, string AppUserId);

        /// <summary>
        /// Affiche les détails d'un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à afficher.</param>
        /// <returns>Une tâche l'objet incident.</returns>
        Task<IncidentDto> GetIncidentDetailsAsync(int incidentId);

        /// <summary>
        /// Récupère la liste des incidents enregistré dans une salle de cinema.
        /// </summary>
        /// /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste d'incident'.</returns>
        Task<List<IncidentDto>> GetIncidentsByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère la liste des incidents enregistré dans toutes les salles de cinema.
        /// </summary>
        /// <returns>Une liste d'incident'.</returns>
        Task<List<IncidentDto>> GetAllIncidentsAsync();
       
        /// <summary>
        /// Ajoute une image à un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="imageUrl">L'URL de l'image à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> AddImageToIncidentAsync(int incidentId, string imageUrl);

        /// <summary>
        /// Supprime une image d'un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="imageUrl">L'URL de l'image à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> RemoveImageFromIncidentAsync(int incidentId, string imageUrl);


        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        Task<List<IncidentDto>> GetTheaterIncidentsAsync(int theaterId);

        /// <summary>
        /// Met à jour le statut d'un incident.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        Task<string> UpdateIncidentStatusAsync(int incidentId, IncidentStatus status, string AppUserId);

        /// <summary>
        /// Met à jour les informations d'un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à mettre à jour.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <param name="resolvedAt">La date de résolution de l'incident (optionnelle).</param>
        /// <param name="imageUrls">La liste des URLs des images associées à l'incident.</param>
        /// <returns>Une réponse indiquant si la mise à jour a réussi.</returns>
        Task<string> UpdateIncidentAsync(UpdateIncidentDto updateIncidentDto);

        /// <summary>
        /// Supprime un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        Task<string> DeleteIncidentAsync(int incidentId);
    }
}
