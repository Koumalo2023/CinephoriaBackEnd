using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles sous forme de DTO.</returns>
        [HttpGet("available/{sessionId}")]
        public async Task<IActionResult> GetAvailableSeats(int sessionId)
        {
            try
            {
                var result = await _seatService.GetAvailableSeatsAsync(sessionId);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la récupération des sièges disponibles.");
            }
        }

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="dto">Les données du siège à ajouter.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpPost("handicap-add-seat")]
        public async Task<IActionResult> AddHandicapSeat([FromBody] AddHandicapSeatDto dto)
        {
            try
            {
                var result = await _seatService.AddHandicapSeatAsync(dto.TheaterId, dto.SeatNumber);
                if (result)
                {
                    return Ok(new { Message = result });
                }
                else
                {
                    return BadRequest("L'ajout du siège réservé pour personnes à mobilité réduite a échoué.");
                }
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de l'ajout du siège réservé pour personnes à mobilité réduite.");
            }
        }

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="dto">Les données du siège à supprimer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpDelete("handicap-delete-seat")]
        public async Task<IActionResult> RemoveHandicapSeat([FromBody] RemoveHandicapSeatDto dto)
        {
            try
            {
                var result = await _seatService.RemoveHandicapSeatAsync(dto.TheaterId, dto.SeatNumber);
                if (result)
                {
                    return Ok("Siège réservé pour personnes à mobilité réduite supprimé avec succès.");
                }
                else
                {
                    return BadRequest("La suppression du siège réservé pour personnes à mobilité réduite a échoué.");
                }
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la suppression du siège réservé pour personnes à mobilité réduite.");
            }
        }

        /// <summary>
        /// Récupère la liste des sièges d'une salle donnée.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>Une liste de sièges.</returns>
        [HttpGet("theater/{theaterId}")]
        public async Task<IActionResult> GetSeatsByTheaterId(int theaterId)
        {
            try
            {
                var seats = await _seatService.GetSeatsByTheaterIdAsync(theaterId);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur s'est produite.");
            }
        }

        /// <summary>
        /// Met à jour les informations d'un siège existant.
        /// </summary>
        /// <param name="updateSeatDto">Les nouvelles informations du siège.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSeat([FromBody] UpdateSeatDto updateSeatDto)
        {
            try
            {
                var result = await _seatService.UpdateSeatAsync(updateSeatDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite.");
            }
        }
    }
}
