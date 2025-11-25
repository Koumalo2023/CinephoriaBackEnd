using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using SkiaSharp;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Repository
{
    public interface IIncidentRepository : IReadRepository<Incident>, IWriteRepository<Incident>
    {
        /// <summary>
        /// Signale un nouvel incident dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="description">La description de l'incident.</param>
        /// <param name="reportedBy">Le nom de la personne ayant signalé l'incident.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ReportIncidentAsync(Incident incident);

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents.</returns>
        Task<List<Incident>> GetTheaterIncidentsAsync(int theaterId);

        /// <summary>
        /// Récupère la liste des incidents enregistré dans une salle de cinema.
        /// </summary>
        /// /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste d'incident'.</returns>
        Task<List<Incident>> GetIncidentsByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère la liste des incidents enregistré dans toutes les salles de cinema.
        /// </summary>
        /// <returns>Une liste d'incident'.</returns>
        Task<List<Incident>> GetAllIncidentsAsync();

        /// <summary>
        /// Met à jour le statut d'un incident.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateIncidentStatusAsync(int incidentId, IncidentStatus status, string AppUserId);

        /// <summary>
        /// Met à jour les informations d'un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à mettre à jour.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <param name="resolvedAt">La date de résolution de l'incident (optionnelle).</param>
        /// <param name="imageUrls">La liste des URLs des images associées à l'incident.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateIncidentAsync(int incidentId, IncidentStatus status, DateTime? resolvedAt, List<string> imageUrls);

        /// <summary>
        /// Affiche les détails d'un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à afficher.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task<Incident> GetByIdAsync(int incidentId);

        /// <summary>
        /// Supprime un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteIncidentAsync(int incidentId);
    }


    public class IncidentRepository : EFRepository<Incident>, IIncidentRepository
    {
        private readonly DbContext _context;

        public IncidentRepository(DbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Signale un nouvel incident dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="description">La description de l'incident.</param>
        /// <param name="reportedBy">L'identifiant de l'employé ayant signalé l'incident.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task ReportIncidentAsync(Incident incident)
        {
            incident.CreatedAt = DateTime.UtcNow;
            incident.UpdatedAt = DateTime.UtcNow;

            _context.Set<Incident>().Add(incident);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents.</returns>
        public async Task<List<Incident>> GetTheaterIncidentsAsync(int theaterId)
        {
            return await _context.Set<Incident>()
                .Include(i => i.Theater) 
                .Include(i => i.ReportedBy)
                .Where(i => i.TheaterId == theaterId)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère la liste des incidents enregistré dans une salle de cinema.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinema.</param>
        /// <returns>Une liste d'incident'.</returns>
        public async Task<List<Incident>> GetIncidentsByCinemaAsync(int cinemaId)
        {
            return await _context.Set<Incident>()
                .Include(i => i.Theater) 
                .Include(i => i.ReportedBy)
                .Where(i => i.Theater.CinemaId == cinemaId)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère la liste des incidents enregistré dans toutes les salles de cinema.
        /// </summary>
        /// <returns>Une liste d'incident'.</returns>
        public async Task<List<Incident>> GetAllIncidentsAsync()
        {
            return await _context.Set<Incident>()
                .Include(i => i.Theater)
                .Include(i => i.ReportedBy)
                .ToListAsync();
        }

        /// <summary>
        /// Affiche les détails d'un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à afficher.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task<Incident> GetByIdAsync(int incidentId)
        {
            return await _context.Set<Incident>()
                .Include(i => i.Theater) // Inclure les détails de la salle
                .Include(i => i.ReportedBy) // Inclure les détails de l'utilisateur qui a signalé l'incident
                .FirstOrDefaultAsync(i => i.IncidentId == incidentId);
        }

        /// <summary>
        /// Met à jour le statut d'un incident.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateIncidentStatusAsync(int incidentId, IncidentStatus status, string AppUserId)
        {
            var incident = await _context.Set<Incident>().FindAsync(incidentId);
            if (incident == null)
            {
                throw new ArgumentException("Incident non trouvé.");
            }

            incident.Status = status;
            incident.UpdatedAt = DateTime.UtcNow;
            incident.ResolvedById = AppUserId;

            if (status == IncidentStatus.Resolved)
            {
                incident.ResolvedAt = DateTime.UtcNow;
            }

            _context.Set<Incident>().Update(incident);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour les informations d'un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à mettre à jour.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <param name="resolvedAt">La date de résolution de l'incident (optionnelle).</param>
        /// <param name="imageUrls">La liste des URLs des images associées à l'incident.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateIncidentAsync(int incidentId, IncidentStatus status, DateTime? resolvedAt, List<string> imageUrls)
        {
            // Récupérer l'incident existant
            var incident = await _context.Set<Incident>().FindAsync(incidentId);
            if (incident == null)
            {
                throw new ArgumentException("Incident non trouvé.");
            }

            // Mettre à jour les propriétés de l'incident
            incident.Status = status;
            incident.ResolvedAt = resolvedAt;
            incident.ImageUrls = imageUrls ?? new List<string>(); // Assure que la liste n'est pas null
            incident.UpdatedAt = DateTime.UtcNow;

            // Marquer l'entité comme modifiée
            _context.Set<Incident>().Update(incident);

            // Sauvegarder les modifications
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteIncidentAsync(int incidentId)
        {
            var incident = await _context.Set<Incident>().FindAsync(incidentId);
            if (incident == null)
            {
                throw new ArgumentException("Incident non trouvé.");
            }

            _context.Set<Incident>().Remove(incident);
            await _context.SaveChangesAsync();
        }
    }
}
