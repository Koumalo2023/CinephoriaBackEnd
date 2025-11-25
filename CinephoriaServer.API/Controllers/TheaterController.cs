using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services; 
using Microsoft.AspNetCore.Mvc; 

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        /// <summary>
        /// Récupère la liste des salles de cinéma associées à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de salles sous forme de DTO.</returns>
        [HttpGet("by-cinema/{cinemaId}")]
        public async Task<IActionResult> GetTheatersByCinema(int cinemaId)
        {
            try
            {
                var result = await _theaterService.GetTheatersByCinemaAsync(cinemaId);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la récupération des salles de cinéma.");
            }
        }

        /// <summary>
        /// Récupère la liste de tous les théâtres.
        /// </summary>
        /// <returns>Une liste de tous les théâtres sous forme de DTO.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTheaters()
        {
            try
            {
                var result = await _theaterService.GetAllTheatersAsync();
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la récupération de tous les théâtres.");
            }
        }

        /// <summary>
        /// Récupère une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>La salle correspondante sous forme de DTO.</returns>
        [HttpGet("{theaterId}")]
        public async Task<IActionResult> GetTheaterById(int theaterId)
        {
            try
            {
                var result = await _theaterService.GetTheaterByIdAsync(theaterId);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la récupération de la salle de cinéma.");
            }
        }

        /// <summary>
        /// Crée une nouvelle salle de cinéma NB: Le nom de salle doit contenir un suffixe ex "Salle A" ou "Salle B".
        /// </summary>
        /// <param name="createTheaterDto">Les données de la salle à créer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTheater([FromBody] CreateTheaterDto createTheaterDto)
        {
            try
            {
                var result =  await _theaterService.CreateTheaterAsync(createTheaterDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la création de la salle de cinéma.");
            }
        }

        /// <summary>
        /// Met à jour les informations d'une salle de cinéma existante.
        /// </summary>
        /// <param name="updateTheaterDto">Les données de la salle à mettre à jour.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTheater([FromBody] UpdateTheaterDto updateTheaterDto)
        {
            try
            {
                var result = await _theaterService.UpdateTheaterAsync(updateTheaterDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la mise à jour de la salle de cinéma.");
            }
        }

        /// <summary>
        /// Supprime une salle de cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpDelete("delete/{theaterId}")]
        public async Task<IActionResult> DeleteTheater(int theaterId)
        {
            try
            {
                var result = await _theaterService.DeleteTheaterAsync(theaterId);
                if (result)
                {
                    return Ok(new { Message = result });
                }
                else
                {
                    return BadRequest("La suppression de la salle de cinéma a échoué.");
                }
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la suppression de la salle de cinéma.");
            }
        }

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        [HttpGet("{theaterId}/incidents")]
        public async Task<IActionResult> GetTheaterIncidents(int theaterId)
        {
            try
            {
                var result = await _theaterService.GetTheaterIncidentsAsync(theaterId);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur inattendue s'est produite lors de la récupération des incidents de la salle de cinéma.");
            }
        }
    }
}
