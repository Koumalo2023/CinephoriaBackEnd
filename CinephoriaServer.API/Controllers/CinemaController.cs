using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CinemasController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        /// <summary>
        /// Crée un nouveau cinéma.
        /// </summary>
        /// <param name="createCinemaDto">Les données du cinéma à créer.</param>
        /// <returns>Le cinéma créé.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateCinema([FromBody] CreateCinemaDto createCinemaDto)
        {
            try
            {
                var result = await _cinemaService.CreateCinemaAsync(createCinemaDto);
                return Ok( new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la création du cinéma." });
            }
        }

        /// <summary>
        /// Récupère la liste de tous les cinémas.
        /// </summary>
        /// <returns>Une liste de cinémas.</returns>
        [HttpGet("cinemas")]
        public async Task<IActionResult> GetAllCinemas()
        {
            try
            {
                var cinemas = await _cinemaService.GetAllCinemasAsync();
                return Ok(cinemas);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération des cinémas." });
            }
        }

        /// <summary>
        /// Récupère un cinéma par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du cinéma.</param>
        /// <returns>Le cinéma correspondant.</returns>
        [HttpGet("cinema/{cinemaId}")]
        public async Task<IActionResult> GetCinemaById(int cinemaId)
        {
            try
            {
                var cinemaDto = await _cinemaService.GetCinemaByIdAsync(cinemaId);
                return Ok(cinemaDto);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération du cinéma." });
            }
        }

        /// <summary>
        /// Met à jour les informations d'un cinéma existant.
        /// </summary>
        /// <param name="updateCinemaDto">Les données du cinéma à mettre à jour.</param>
        /// <returns>Le cinéma mis à jour.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateCinema([FromBody] UpdateCinemaDto updateCinemaDto)
        {
            try
            {
                var result = await _cinemaService.UpdateCinemaAsync(updateCinemaDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la mise à jour du cinéma." });
            }
        }

        /// <summary>
        /// Supprime un cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Une réponse indiquant si la suppression a réussi.</returns>
        [HttpDelete("cinema/{cinemaId}")]
        public async Task<IActionResult> DeleteCinema(int cinemaId)
        {
            try
            {
                var result = await _cinemaService.DeleteCinemaAsync(cinemaId);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la suppression du cinéma." });
            }
        }
    }
}
