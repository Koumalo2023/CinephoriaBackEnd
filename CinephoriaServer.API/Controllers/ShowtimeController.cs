using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.DTOs.Showtime;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly IShowtimeStatusService _showtimeStatusService;
        private readonly ILogger<ShowtimeController> _logger;

        public ShowtimeController(
            IShowtimeService showtimeService,
            IShowtimeStatusService showtimeStatusService,
            ILogger<ShowtimeController> logger)
        {
            _showtimeService = showtimeService;
            _showtimeStatusService = showtimeStatusService;
            _logger = logger;
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeDto createShowtimeDto)
        {
            try
            {
                var result = await _showtimeService.CreateShowtimeAsync(createShowtimeDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite.");
            }
        }

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="updateShowtimeDto">Les données mises à jour de la séance.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateShowtime([FromBody] UpdateShowtimeDto updateShowtimeDto)
        {
            try
            {
                var result =await _showtimeService.UpdateShowtimeAsync(updateShowtimeDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite.");
            }
        }

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("delete/{showtimeId}")]
        public async Task<IActionResult> DeleteShowtime(int showtimeId)
        {
            try
            {
                var result = await _showtimeService.DeleteShowtimeAsync(showtimeId);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite.");
            }
        }

        /// <summary>
        /// Récupère la liste de toutes les séances.
        /// </summary>
        /// <returns>Une liste de séances sous forme de DTO.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllShowtimes()
        {
            try
            {
                var showtimes = await _showtimeService.GetAllShowtimesAsync();
                return Ok(showtimes);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite.");
            }
        }

        /// <summary>
        /// Récupère les détails d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Les détails de la séance sous forme de DTO.</returns>
        [HttpGet("{showtimeId}")]
        public async Task<IActionResult> GetShowtimeDetails(int showtimeId)
        {
            try
            {
                var showtimeDetails = await _showtimeService.GetShowtimeDetailsAsync(showtimeId);
                return Ok(showtimeDetails);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite.");
            }
        }

        #region Showtime Status Methods

        /// <summary>
        /// Récupère toutes les séances avec leurs statuts
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeStatusDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllShowtimesWithStatus()
        {
            try
            {
                var showtimes = await _showtimeStatusService.GetAllShowtimesWithStatusAsync();
                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances avec statuts");
                return BadRequest(new { message = "Erreur lors de la récupération des séances" });
            }
        }

        /// <summary>
        /// Récupère les séances par statut
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeStatusDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetShowtimesByStatus(EnumConfig.ShowtimeStatus status)
        {
            try
            {
                var showtimes = await _showtimeStatusService.GetShowtimesByStatusAsync(status);
                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances par statut: {Status}", status);
                return BadRequest(new { message = $"Erreur lors de la récupération des séances avec statut {status}" });
            }
        }

        /// <summary>
        /// Récupère les séances à venir (prochaines 24h)
        /// </summary>
        [HttpGet("upcoming")]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeStatusDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUpcomingShowtimes()
        {
            try
            {
                var showtimes = await _showtimeStatusService.GetUpcomingShowtimesAsync();
                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances à venir");
                return BadRequest(new { message = "Erreur lors de la récupération des séances à venir" });
            }
        }

        /// <summary>
        /// Récupère les séances en cours
        /// </summary>
        [HttpGet("ongoing")]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeStatusDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetOngoingShowtimes()
        {
            try
            {
                var showtimes = await _showtimeStatusService.GetOngoingShowtimesAsync();
                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances en cours");
                return BadRequest(new { message = "Erreur lors de la récupération des séances en cours" });
            }
        }

        /// <summary>
        /// Récupère les statistiques des statuts des séances
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ShowtimeStatusStatsDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetShowtimeStats()
        {
            try
            {
                var stats = await _showtimeStatusService.GetShowtimeStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques des séances");
                return BadRequest(new { message = "Erreur lors de la récupération des statistiques" });
            }
        }

        /// <summary>
        /// Met à jour manuellement le statut d'une séance
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateShowtimeStatus(int id, [FromBody] ShowtimeStatusUpdateDto updateDto)
        {
            try
            {
                var result = await _showtimeStatusService.UpdateShowtimeStatusAsync(id, updateDto.Status);
                if (!result)
                {
                    return NotFound(new { message = "Séance non trouvée" });
                }

                return Ok(new { message = "Statut de la séance mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de la séance {Id}", id);
                return BadRequest(new { message = "Erreur lors de la mise à jour du statut" });
            }
        }

        /// <summary>
        /// Force la mise à jour automatique des statuts
        /// </summary>
        [HttpPost("update-status")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ForceStatusUpdate()
        {
            try
            {
                var updatedCount = await _showtimeStatusService.UpdateAllShowtimeStatusesAsync();
                return Ok(new {
                    message = "Mise à jour des statuts effectuée avec succès",
                    updatedCount = updatedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour forcée des statuts");
                return BadRequest(new { message = "Erreur lors de la mise à jour des statuts" });
            }
        }

        /// <summary>
        /// Récupère les films ayant au moins une séance
        /// </summary>
        [HttpGet("movies-with-showtimes")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMoviesWithShowtimes()
        {
            try
            {
                var movies = await _showtimeStatusService.GetMoviesWithShowtimesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des films avec séances");
                return BadRequest(new { message = "Erreur lors de la récupération des films avec séances" });
            }
        }

        /// <summary>
        /// Récupère les films ajoutés le dernier mercredi avec leurs séances
        /// </summary>
        [HttpGet("recent-movies")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRecentMoviesWithShowtimes()
        {
            try
            {
                var movies = await _showtimeStatusService.GetRecentMoviesWithShowtimesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des films récents avec séances");
                return BadRequest(new { message = "Erreur lors de la récupération des films récents avec séances" });
            }
        }

        #endregion
    }
}
