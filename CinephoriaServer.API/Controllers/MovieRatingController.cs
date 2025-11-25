using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieRatingController : ControllerBase
    {
        private readonly IMovieRatingService _movieRatingService;

        public MovieRatingController(IMovieRatingService movieRatingService)
        {
            _movieRatingService = movieRatingService;
        }

        /// <summary>
        /// Soumet un nouvel avis sur un film.
        /// </summary>
        /// <param name="createMovieRatingDto">Les données de l'avis à soumettre.</param>
        /// <returns>Une réponse indiquant si l'avis a été soumis avec succès.</returns>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitMovieReview([FromBody] CreateMovieRatingDto createMovieRatingDto)
        {
            try
            {
                // Récupérer l'identifiant de l'utilisateur connecté
                var AppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(AppUserId))
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                // Appeler la méthode SubmitMovieReviewAsync avec l'identifiant de l'utilisateur connecté
                var result = await _movieRatingService.SubmitMovieReviewAsync(createMovieRatingDto, AppUserId);

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
        /// Valide un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une réponse indiquant si l'avis a été validé avec succès.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPut("validate/{reviewId}")]
        public async Task<IActionResult> ValidateReview(int reviewId)
        {
            try
            {
                var result = await _movieRatingService.ValidateReviewAsync(reviewId);
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
        /// Supprime un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une réponse indiquant si l'avis a été supprimé avec succès.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpDelete("delete/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var result = await _movieRatingService.DeleteReviewAsync(reviewId);
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
        /// Récupère la liste des avis associés à un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis sous forme de DTO.</returns>
        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetMovieReviews(int movieId)
        {
            try
            {
                var reviews = await _movieRatingService.GetMovieReviewsAsync(movieId);
                return Ok(reviews);
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
        /// Récupère les détails d'un avis spécifique.
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis.</param>
        /// <returns>Les détails de l'avis sous forme de DTO.</returns>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewDetails(int reviewId)
        {
            try
            {
                var reviewDetails = await _movieRatingService.GetReviewDetailsAsync(reviewId);
                return Ok(reviewDetails);
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
        /// Met à jour un avis existant.
        /// </summary>
        /// <param name="updateMovieRatingDto">Les données mises à jour de l'avis.</param>
        /// <returns>Une réponse indiquant si l'avis a été mis à jour avec succès.</returns>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateMovieRatingDto updateMovieRatingDto)
        {
            try
            {
                var result = await _movieRatingService.UpdateReviewAsync(updateMovieRatingDto);
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
