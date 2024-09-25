using CinephoriaBackEnd.Models;
using MongoDB.Driver;

namespace CinephoriaBackEnd.Repository
{
    /// <summary>
    /// Interface pour la gestion des opérations CRUD sur les incidents dans MongoDB.
    /// </summary>
    public interface IIncidentRepository
    {
        /// <summary>
        /// Récupérer tous les incidents signalés.
        /// </summary>
        /// <returns>Liste de tous les incidents dans la base de données MongoDB.</returns>
        Task<List<Incident>> GetAllIncidents();

        /// <summary>
        /// Signaler un nouvel incident dans le système.
        /// </summary>
        /// <param name="incident">Les détails de l'incident à créer.</param>
        /// <returns>L'incident nouvellement créé.</returns>
        Task<Incident> CreateIncident(Incident incident);

        /// <summary>
        /// Récupérer les détails d'un incident spécifique à partir de son identifiant unique.
        /// </summary>
        /// <param name="id">L'identifiant unique de l'incident.</param>
        /// <returns>Les détails de l'incident correspondant à l'ID.</returns>
        Task<Incident> GetIncidentById(string id);

        /// <summary>
        /// Mettre à jour les informations d'un incident existant.
        /// </summary>
        /// <param name="incident">L'incident contenant les informations mises à jour.</param>
        Task UpdateIncident(Incident incident);

        /// <summary>
        /// Supprimer un incident spécifique de la base de données MongoDB.
        /// </summary>
        /// <param name="id">L'identifiant unique de l'incident à supprimer.</param>
        Task DeleteIncident(string id);

        /// <summary>
        /// Récupérer tous les incidents liés à une salle spécifique à partir de l'identifiant de la salle.
        /// </summary>
        /// <param name="roomId">L'ID de la salle concernée.</param>
        /// <returns>Liste des incidents signalés pour cette salle.</returns>
        Task<List<Incident>> GetIncidentsByRoomId(int roomId);
    }


    public class IncidentRepository : IIncidentRepository
    {
        private readonly IMongoCollection<Incident> _incidents;

        public IncidentRepository(MongoDbContext context)
        {
            _incidents = context.Incidents;
        }

        public async Task<List<Incident>> GetAllIncidents()
        {
            return await _incidents.Find(incident => true).ToListAsync();
        }

        public async Task<Incident> CreateIncident(Incident incident)
        {
            await _incidents.InsertOneAsync(incident);
            return incident;
        }

        public async Task<Incident> GetIncidentById(string id)
        {
            return await _incidents.Find(incident => incident.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateIncident(Incident incident)
        {
            await _incidents.ReplaceOneAsync(i => i.Id == incident.Id, incident);
        }

        public async Task DeleteIncident(string id)
        {
            await _incidents.DeleteOneAsync(i => i.Id == id);
        }

        public async Task<List<Incident>> GetIncidentsByRoomId(int roomId)
        {
            return await _incidents.Find(incident => incident.RoomId == roomId).ToListAsync();
        }
    }

}
