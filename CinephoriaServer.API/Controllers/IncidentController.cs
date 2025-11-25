using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinephoriaServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IImageService _imageService;

        public IncidentController(IIncidentService incidentService, IImageService imageService)
        {
            _incidentService = incidentService;
            _imageService = imageService;
        }

        /// <summary>
        /// Signale un nouvel incident dans une salle de cinéma.
        /// </summary>
        /// <param name="createIncidentDto">Les données de l'incident à signaler.</param>
        /// <returns>Une réponse indiquant si l'incident a été signalé avec succès.</returns>
        [Authorize]
        [HttpPost("report")]
        public async Task<IActionResult> ReportIncident([FromBody] CreateIncidentDto createIncidentDto)
        {
            try
            {
                // Récupérer l'identifiant de l'utilisateur connecté
                var AppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(AppUserId))
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                // Appeler la méthode ReportIncidentAsync avec l'identifiant de l'utilisateur connecté
                var result = await _incidentService.ReportIncidentAsync(createIncidentDto, AppUserId);

                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère les détails d'un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident.</param>
        /// <returns>Les détails de l'incident sous forme de DTO.</returns>
        [HttpGet("{incidentId}")]
        public async Task<IActionResult> GetIncidentDetailsAsync(int incidentId)
        {
            try
            {
                var incidentDetails = await _incidentService.GetIncidentDetailsAsync(incidentId);
                return Ok(incidentDetails);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des incidents associés à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        [HttpGet("cinema/{cinemaId}")]
        public async Task<IActionResult> GetIncidentsByCinemaAsync(int cinemaId)
        {
            try
            {
                var incidents = await _incidentService.GetIncidentsByCinemaAsync(cinemaId);
                return Ok(incidents);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère tous les incidents dans tous les cinémas.
        /// </summary>
        /// <returns>Une liste de tous les incidents sous forme de DTO.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllIncidentsAsync()
        {
            try
            {
                var incidents = await _incidentService.GetAllIncidentsAsync();
                return Ok(incidents);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        // Upload image d'un incident
        [HttpPost("upload-incident-image/{incidentId}")]
        public async Task<IActionResult> UploadIncidentImage(int incidentId, [FromForm] IFormFile file)
        {
            try
            {
                string folder = "incidents";
                var imageUrl = await _imageService.UploadImageAsync(file, folder);
                if (imageUrl == null)
                {
                    throw new ApiException("Erreur lors du téléchargement de l'image.", StatusCodes.Status400BadRequest);
                }

                var result = await _incidentService.AddImageToIncidentAsync(incidentId, imageUrl);

                return Ok(new { Url = imageUrl, Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents.</returns>
        [HttpGet("theater/{theaterId}")]
        public async Task<IActionResult> GetTheaterIncidents(int theaterId)
        {
            try
            {
                var incidents = await _incidentService.GetTheaterIncidentsAsync(theaterId);
                return Ok(incidents);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Met à jour le statut d'un incident.
        /// </summary>
        /// <param name="incidentStatusUpdateDto">Les données de mise à jour du statut de l'incident.</param>
        /// <returns>Une réponse indiquant si la mise à jour a réussi.</returns>
        [HttpPut("status")]
        public async Task<IActionResult> UpdateIncidentStatus([FromBody] IncidentStatusUpdateDto incidentStatusUpdateDto)
        {
            try
            {
                // Récupérer l'identifiant de l'utilisateur connecté
                var AppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(AppUserId))
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                var result = await _incidentService.UpdateIncidentStatusAsync(
                    incidentStatusUpdateDto.IncidentId,
                    incidentStatusUpdateDto.Status, AppUserId
                );

                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Met à jour les informations d'un incident existant.
        /// </summary>
        /// <param name="updateIncidentDto">Les données de l'incident à mettre à jour.</param>
        /// <returns>Une réponse indiquant si la mise à jour a réussi.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateIncident([FromBody] UpdateIncidentDto updateIncidentDto)
        {
            try
            {
                var result = await _incidentService.UpdateIncidentAsync(updateIncidentDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime un incident en fonction de son identifiant.
        /// </summary>
        /// <param name="incidentId">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Une réponse indiquant si la suppression a réussi.</returns>
        [HttpDelete("{incidentId}")]
        public async Task<IActionResult> DeleteIncident(int incidentId)
        {
            try
            {
                var result = await _incidentService.DeleteIncidentAsync(incidentId);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }
    }
}
