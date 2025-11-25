using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentService> _logger;
        private readonly IImageService _imageService;

        public IncidentService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<IncidentService> logger, IImageService imageService, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
            _userManager = userManager; 
        }


        /// Signale un nouvel incident dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="description">La description de l'incident.</param>
        /// <param name="reportedBy">L'identifiant de l'employé ayant signalé l'incident.</param>
        /// <param name="imageUrls">Liste des URLs des images associées à l'incident.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<string> ReportIncidentAsync(CreateIncidentDto createIncidentDto, string AppUserId)
        {
            if (createIncidentDto.TheaterId <= 0 || string.IsNullOrWhiteSpace(createIncidentDto.Description) || string.IsNullOrWhiteSpace(AppUserId))
            {
                throw new ApiException("Entrées invalides.", StatusCodes.Status400BadRequest);
            }

            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null) throw new ApiException("Utilisateur introuvable.", StatusCodes.Status400BadRequest);

            var theaterExists = await _unitOfWork.Theaters.GetByIdAsync(createIncidentDto.TheaterId) != null;
            if (!theaterExists) throw new ApiException("Salle introuvable.", StatusCodes.Status400BadRequest);


            _logger.LogInformation("Mapping du DTO...");


            var incident = _mapper.Map<Incident>(createIncidentDto);
            if (incident == null) throw new Exception("Erreur de mapping.");

            incident.ResolvedById = null;

            incident.ReportedAt = DateTime.UtcNow;
            incident.Status = IncidentStatus.Pending;

            await _unitOfWork.Incidents.ReportIncidentAsync(incident);
            await _unitOfWork.CompleteAsync();


            _logger.LogInformation("Incident signalé avec succès.");

            return "L'incident a été signalé avec succès.";
        }

        /// <summary>
        /// Affiche les détails d'un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à afficher.</param>
        /// <returns>Une tâche l'objet incident.</returns>
        public async Task<IncidentDto> GetIncidentDetailsAsync(int incidentId)
        {
            var incident = await _unitOfWork.Incidents.GetByIdAsync(incidentId);
            if (incident == null)
            {
                throw new ApiException("Incident non trouvé.", StatusCodes.Status404NotFound);
            }

            // Mapper l'entité Incident vers IncidentDto
            var incidentDto = _mapper.Map<IncidentDto>(incident);
            return incidentDto;
        }

        /// <summary>
        /// Récupère la liste des incidents enregistré dans une salle de cinema.
        /// </summary>
        /// /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste d'incident'.</returns>
        public async Task<List<IncidentDto>> GetIncidentsByCinemaAsync(int cinemaId)
        {
            var incidents = await _unitOfWork.Incidents.GetIncidentsByCinemaAsync(cinemaId);

            // Mapper la liste des entités Incident vers une liste de IncidentDto
            var incidentDtos = _mapper.Map<List<IncidentDto>>(incidents);
            return incidentDtos;
        }

        /// <summary>
        /// Récupère la liste des incidents enregistré dans toutes les salles de cinema.
        /// </summary>
        /// <returns>Une liste d'incident'.</returns>
        public async Task<List<IncidentDto>> GetAllIncidentsAsync()
        {
            var incidents = await _unitOfWork.Incidents.GetAllIncidentsAsync();

            // Mapper la liste des entités Incident vers une liste de IncidentDto
            var incidentDtos = _mapper.Map<List<IncidentDto>>(incidents);
            return incidentDtos;
        }

        /// <summary>
        /// Ajoute une image à un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="imageUrl">L'URL de l'image à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> AddImageToIncidentAsync(int incidentId, string imageUrl)
        {
            // Récupérer l'incident existant
            var incident = await _unitOfWork.Incidents.GetByIdAsync(incidentId);
            if (incident == null)
            {
                throw new ApiException("Incident non trouvé.", StatusCodes.Status404NotFound);
            }

            // Ajouter l'URL de l'image à la liste des images de l'incident
            incident.ImageUrls.Add(imageUrl);

            // Mettre à jour l'incident dans la base de données
            await _unitOfWork.Incidents.UpdateAsync(incident);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Image ajoutée avec succès à l'incident avec l'ID {IncidentId}.", incidentId);
            return "Image ajoutée avec succès à l'incident";
        }

        /// <summary>
        /// Supprime une image d'un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="imageUrl">L'URL de l'image à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> RemoveImageFromIncidentAsync(int incidentId, string imageUrl)
        {
            // Récupérer l'incident existant
            var incident = await _unitOfWork.Incidents.GetByIdAsync(incidentId);
            if (incident == null)
            {
                throw new ApiException("Incident non trouvé.", StatusCodes.Status404NotFound);
            }

            // Supprimer l'image du stockage
            var imageDeleted = await _imageService.DeleteImageAsync(imageUrl);
            if (!imageDeleted)
            {
                throw new ApiException("L'image n'a pas pu être supprimée du stockage.", StatusCodes.Status500InternalServerError);
            }

            // Supprimer l'URL de l'image de la liste des images de l'incident
            incident.ImageUrls.Remove(imageUrl);

            // Mettre à jour l'incident dans la base de données
            await _unitOfWork.Incidents.UpdateAsync(incident);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Image supprimée avec succès de l'incident avec l'ID {IncidentId}.", incidentId);
            return "Image supprimée avec succès de l'incident.";
        }


        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        public async Task<List<IncidentDto>> GetTheaterIncidentsAsync(int theaterId)
        {
            if (theaterId <= 0)
            {
                throw new ApiException("L'identifiant de la salle doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            // Récupérer les incidents
            var incidents = await _unitOfWork.Incidents.GetTheaterIncidentsAsync(theaterId);
            var incidentDtos = _mapper.Map<List<IncidentDto>>(incidents);

            _logger.LogInformation("{Count} incidents récupérés pour la salle avec l'ID {TheaterId}.", incidentDtos.Count, theaterId);
            return incidentDtos;
        }

        /// <summary>
        /// Met à jour le statut d'un incident.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<string> UpdateIncidentStatusAsync(int incidentId, IncidentStatus status, string AppUserId)
        {
            if (incidentId <= 0)
            {
                throw new ApiException("L'identifiant de l'incident doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            if (string.IsNullOrWhiteSpace(AppUserId))
            {
                throw new ApiException("L'identifiant de l'utilisateur connecté est manquant.", StatusCodes.Status400BadRequest);
            }

            var incident = await _unitOfWork.Incidents.GetByIdAsync(incidentId);
            if (incident == null)
            {
                throw new ApiException("Incident introuvable.", StatusCodes.Status404NotFound);
            }

            incident.Status = status;

            if (status == IncidentStatus.Resolved)
            {
                incident.ResolvedById = AppUserId; // Identifiant de l'utilisateur qui résout l'incident.
                incident.ResolvedAt = DateTime.UtcNow; // Date et heure de la résolution.
            }

            await _unitOfWork.Incidents.UpdateAsync(incident);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Statut de l'incident avec l'ID {IncidentId} mis à jour avec succès.", incidentId);

            return "Statut de l'incident modifié avec succès";
        }


        /// <summary>
        /// Met à jour les informations d'un incident existant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à mettre à jour.</param>
        /// <param name="status">Le nouveau statut de l'incident.</param>
        /// <param name="resolvedAt">La date de résolution de l'incident (optionnelle).</param>
        /// <param name="imageUrls">La liste des URLs des images associées à l'incident.</param>
        /// <returns>Une réponse indiquant si la mise à jour a réussi.</returns>
        public async Task<string> UpdateIncidentAsync(UpdateIncidentDto updateIncidentDto)
        {
            // Validation des entrées
            if (updateIncidentDto.IncidentId <= 0)
            {
                throw new ApiException("L'identifiant de l'incident doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            // Récupérer l'incident existant
            var incident = await _unitOfWork.Incidents.GetByIdAsync(updateIncidentDto.IncidentId);
            if (incident == null)
            {
                throw new ApiException("Incident non trouvé.", StatusCodes.Status404NotFound);
            }

            // Mapper les propriétés du DTO vers l'entité existante
            _mapper.Map(updateIncidentDto, incident);

            // Mettre à jour l'incident dans la base de données
            await _unitOfWork.Incidents.UpdateAsync(incident);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Incident avec l'ID {IncidentId} mis à jour avec succès.", updateIncidentDto.IncidentId);
            return "Incident mis à jour avec succès.";
        }

        /// <summary>
        /// Supprime un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<string> DeleteIncidentAsync(int incidentId)
        {
            if (incidentId <= 0)
            {
                throw new ApiException("L'identifiant de l'incident doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            // Supprimer l'incident
            await _unitOfWork.Incidents.DeleteIncidentAsync(incidentId);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Incident avec l'ID {IncidentId} supprimé avec succès.", incidentId);
            return "Incident supprimé avec succès";
        }
    }


}
