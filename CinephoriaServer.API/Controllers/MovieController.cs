
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using CinephoriaServer.API.Services.FavoriteMovie; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;
        private readonly IImageService _imageService;
        private readonly IFavoriteMovieService _favoriteMovieService;
        private readonly IEmployeeFavoriteService _employeeFavoriteService;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger,
                              IImageService imageService, IFavoriteMovieService favoriteMovieService,
                              IEmployeeFavoriteService employeeFavoriteService)
        {
            _movieService = movieService;
            _logger = logger;
            _imageService = imageService;
            _favoriteMovieService = favoriteMovieService;
            _employeeFavoriteService = employeeFavoriteService;
        }

        /// <summary>
        /// Récupère la liste des derniers films ajoutés avec au moins une séance.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentMovies()
        {
            try
            {
                var movies = await _movieService.GetRecentMoviesAsync();
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des derniers films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste de tous les films.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération de tous les films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetMovieDetails(int movieId)
        {
            try
            {
                var movieDetails = await _movieService.GetMovieDetailsAsync(movieId);
                
                // Enregistrer la consultation si l'utilisateur est authentifié
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Enregistrer en arrière-plan sans attendre le résultat
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _movieService.RecordMovieViewAsync(userId, movieId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Erreur lors de l'enregistrement de la consultation du film {MovieId} par l'utilisateur {UserId}", movieId, userId);
                            }
                        });
                    }
                }
                
                return Ok(movieDetails);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des détails du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        [HttpGet("{movieId}/sessions")]
        public async Task<IActionResult> GetMovieSessions(int movieId)
        {
            try
            {
                var sessions = await _movieService.GetMovieSessionsAsync(movieId);
                return Ok(sessions);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des séances du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre, de la date, de l'année, du réalisateur, des acteurs et de la classification.
        /// </summary>
        /// <param name="filterDto">Les critères de filtrage.</param>
        /// <returns>Une liste de films correspondant aux critères.</returns>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterMovies([FromBody] FilterMoviesRequestDto filterDto)
        {
            try
            {
                var movies = await _movieService.FilterMoviesAsync(filterDto);
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors du filtrage des films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="reviewDto">Les données de l'avis.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpPost("review")]
        public async Task<IActionResult> SubmitMovieReview([FromBody] MovieReviewDto reviewDto)
        {
            try
            {
                var result = await _movieService.SubmitMovieReviewAsync(reviewDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la soumission de l'avis.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère l'historique des films consultés par l'utilisateur connecté.
        /// </summary>
        /// <param name="limit">Nombre maximum de films à récupérer (optionnel).</param>
        /// <returns>Une liste de films consultés récemment.</returns>
        /// <response code="200">Retourne la liste des films consultés récemment</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpGet("history")]
        public async Task<IActionResult> GetUserMovieHistory([FromQuery] int? limit = null)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                var movieHistory = await _movieService.GetUserMovieHistoryAsync(userId, limit);
                return Ok(movieHistory);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération de l'historique des films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère tous les films qui ont au moins une séance programmée.
        /// </summary>
        /// <returns>Une liste de films avec séances programmées.</returns>
        /// <response code="200">Retourne la liste des films avec séances</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [HttpGet("with-showtimes")]
        public async Task<IActionResult> GetAllMoviesWithShowtime()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesWithShowtimeAsync();
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des films avec séances.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="createMovieDto">Les données du film à créer.</param>
        /// <returns>L'identifiant du film créé.</returns>
        /// <response code="200">Retourne l'identifiant du film créé</response>
        /// <response code="400">Si les données du film sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto createMovieDto)
        {
            try
            {
                var movieId = await _movieService.CreateMovieAsync(createMovieDto);
                return Ok(new { MovieId = movieId, Message = "Film créé avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la création du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Ajoute une affiche à un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le film n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("{movieId}/poster")]
        public async Task<IActionResult> AddPosterToMovie(int movieId, [FromBody] string posterUrl)
        {
            try
            {
                var result = await _movieService.AddPosterToMovieAsync(movieId, posterUrl);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de l'ajout de l'affiche au film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime une affiche d'un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le film n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{movieId}/poster")]
        public async Task<IActionResult> RemovePosterFromMovie(int movieId, [FromBody] string posterUrl)
        {
            try
            {
                var result = await _movieService.RemovePosterFromMovieAsync(movieId, posterUrl);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppression de l'affiche du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="updateMovieDto">Les nouvelles données du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le film n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieDto updateMovieDto)
        {
            try
            {
                var result = await _movieService.UpdateMovieAsync(updateMovieDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la mise à jour du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le film n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{movieId}")]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            try
            {
                var result = await _movieService.DeleteMovieAsync(movieId);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppression du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des films qui ont des séances dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        /// <response code="200">Retourne la liste des films du cinéma</response>
        /// <response code="400">Si l'identifiant du cinéma est invalide</response>
        /// <response code="404">Si le cinéma n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [HttpGet("cinema/{cinemaId}")]
        public async Task<IActionResult> GetMoviesByCinemaId(int cinemaId)
        {
            try
            {
                var movies = await _movieService.GetMoviesByCinemaIdAsync(cinemaId);
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des films par cinéma.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Recherche des films dans The Movie Database (TMDb)
        /// </summary>
        /// <param name="searchRequest">Paramètres de recherche</param>
        /// <returns>Résultats de la recherche TMDb</returns>
        /// <response code="200">Retourne les résultats de la recherche TMDb</response>
        /// <response code="400">Si les paramètres de recherche sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpPost("tmdb/search")]
        public async Task<IActionResult> SearchTMDbMovies([FromBody] TMDbSearchRequestDto searchRequest)
        {
            try
            {
                var result = await _movieService.SearchTMDbMoviesAsync(searchRequest);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la recherche TMDb.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Importe un film depuis The Movie Database (TMDb) dans la base locale
        /// </summary>
        /// <param name="importRequest">Paramètres d'import</param>
        /// <returns>Identifiant du film importé</returns>
        /// <response code="200">Retourne l'identifiant du film importé</response>
        /// <response code="400">Si les paramètres d'import sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le film TMDb n'est pas trouvé</response>
        /// <response code="409">Si le film existe déjà dans la base locale</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("tmdb/import")]
        public async Task<IActionResult> ImportMovieFromTMDb([FromBody] TMDbImportRequestDto importRequest)
        {
            try
            {
                var movieId = await _movieService.ImportMovieFromTMDbAsync(importRequest);
                return Ok(new { MovieId = movieId, Message = "Film importé avec succès depuis TMDb." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de l'import du film depuis TMDb.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        #region Favorite Movie Methods

        /// <summary>
        /// Ajoute un film aux favoris de l'utilisateur connecté.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="404">Si le film n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpPost("favorites/{movieId}")]
        public async Task<IActionResult> AddMovieToFavorites(int movieId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                await _favoriteMovieService.AddMovieToFavoritesAsync(userId, movieId);
                return Ok(new { Message = "Film ajouté aux favoris avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de l'ajout du film aux favoris.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime un film des favoris de l'utilisateur connecté.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="404">Si le film n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpDelete("favorites/{movieId}")]
        public async Task<IActionResult> RemoveMovieFromFavorites(int movieId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                await _favoriteMovieService.RemoveMovieFromFavoritesAsync(userId, movieId);
                return Ok(new { Message = "Film supprimé des favoris avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppression du film des favoris.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des films favoris de l'utilisateur connecté.
        /// </summary>
        /// <returns>Une liste de films favoris.</returns>
        /// <response code="200">Retourne la liste des films favoris</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpGet("favorites")]
        public async Task<IActionResult> GetUserFavoriteMovies()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                var favoriteMovies = await _favoriteMovieService.GetUserFavoriteMoviesAsync(userId);
                return Ok(favoriteMovies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des films favoris.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Vérifie si un film est dans les favoris de l'utilisateur connecté.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est dans les favoris, sinon false.</returns>
        /// <response code="200">Retourne l'état du film dans les favoris</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpGet("favorites/{movieId}/check")]
        public async Task<IActionResult> IsMovieInFavorites(int movieId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                var isInFavorites = await _favoriteMovieService.IsMovieInFavoritesAsync(userId, movieId);
                return Ok(new { IsInFavorites = isInFavorites });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la vérification du film dans les favoris.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        #endregion

        #region Employee Favorite Methods

        /// <summary>
        /// Crée un coup de cœur employé pour un film.
        /// </summary>
        /// <param name="createDto">Les données de création du coup de cœur.</param>
        /// <returns>L'identifiant du coup de cœur créé.</returns>
        /// <response code="200">Retourne l'identifiant du coup de cœur créé</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost("employee-favorites")]
        public async Task<IActionResult> CreateEmployeeFavorite([FromBody] CreateEmployeeFavoriteDto createDto)
        {
            try
            {
                var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(appUserId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                var employeeFavoriteId = await _employeeFavoriteService.CreateEmployeeFavoriteAsync(appUserId, createDto);
                return Ok(new { EmployeeFavoriteId = employeeFavoriteId, Message = "Coup de cœur employé créé avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la création du coup de cœur employé.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Met à jour un coup de cœur employé existant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <param name="updateDto">Les données de mise à jour.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="400">Si les données sont invalides</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le coup de cœur n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPut("employee-favorites/{employeeFavoriteId}")]
        public async Task<IActionResult> UpdateEmployeeFavorite(int employeeFavoriteId, [FromBody] UpdateEmployeeFavoriteDto updateDto)
        {
            try
            {
                await _employeeFavoriteService.UpdateEmployeeFavoriteAsync(employeeFavoriteId, updateDto);
                return Ok(new { Message = "Coup de cœur employé mis à jour avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la mise à jour du coup de cœur employé.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime un coup de cœur employé.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        /// <response code="200">Retourne un message de succès</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="403">Si l'utilisateur n'a pas les droits nécessaires</response>
        /// <response code="404">Si le coup de cœur n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpDelete("employee-favorites/{employeeFavoriteId}")]
        public async Task<IActionResult> DeleteEmployeeFavorite(int employeeFavoriteId)
        {
            try
            {
                await _employeeFavoriteService.DeleteEmployeeFavoriteAsync(employeeFavoriteId);
                return Ok(new { Message = "Coup de cœur employé supprimé avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppression du coup de cœur employé.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère un coup de cœur employé par son identifiant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <returns>Le DTO de réponse du coup de cœur.</returns>
        /// <response code="200">Retourne le coup de cœur employé</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="404">Si le coup de cœur n'est pas trouvé</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("employee-favorites/{employeeFavoriteId}")]
        public async Task<IActionResult> GetEmployeeFavoriteById(int employeeFavoriteId)
        {
            try
            {
                var employeeFavorite = await _employeeFavoriteService.GetEmployeeFavoriteByIdAsync(employeeFavoriteId);
                return Ok(employeeFavorite);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération du coup de cœur employé.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère tous les coups de cœur de l'employé connecté.
        /// </summary>
        /// <returns>Une liste de DTO de réponse des coups de cœur.</returns>
        /// <response code="200">Retourne la liste des coups de cœur de l'employé</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("employee-favorites/my-favorites")]
        public async Task<IActionResult> GetEmployeeFavoritesByUser()
        {
            try
            {
                var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(appUserId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                var employeeFavorites = await _employeeFavoriteService.GetEmployeeFavoritesByUserAsync(appUserId);
                return Ok(employeeFavorites);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des coups de cœur de l'employé.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère tous les coups de cœur pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de DTO de réponse des coups de cœur.</returns>
        /// <response code="200">Retourne la liste des coups de cœur pour le film</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("employee-favorites/movie/{movieId}")]
        public async Task<IActionResult> GetEmployeeFavoritesByMovie(int movieId)
        {
            try
            {
                var employeeFavorites = await _employeeFavoriteService.GetEmployeeFavoritesByMovieAsync(movieId);
                return Ok(employeeFavorites);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des coups de cœur pour le film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Vérifie si l'employé connecté a déjà marqué un film comme coup de cœur.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est déjà marqué comme coup de cœur, sinon false.</returns>
        /// <response code="200">Retourne l'état du film dans les coups de cœur</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("employee-favorites/movie/{movieId}/check")]
        public async Task<IActionResult> HasEmployeeFavorite(int movieId)
        {
            try
            {
                var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(appUserId))
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });

                var hasEmployeeFavorite = await _employeeFavoriteService.HasEmployeeFavoriteAsync(appUserId, movieId);
                return Ok(new { HasEmployeeFavorite = hasEmployeeFavorite });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la vérification du film dans les coups de cœur employé.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère tous les coups de cœur actifs.
        /// </summary>
        /// <returns>Une liste de DTO de réponse des coups de cœur actifs.</returns>
        /// <response code="200">Retourne la liste des coups de cœur actifs</response>
        /// <response code="401">Si l'utilisateur n'est pas authentifié</response>
        /// <response code="500">En cas d'erreur serveur inattendue</response>
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("employee-favorites/active")]
        public async Task<IActionResult> GetAllActiveEmployeeFavorites()
        {
            try
            {
                var employeeFavorites = await _employeeFavoriteService.GetAllActiveEmployeeFavoritesAsync();
                return Ok(employeeFavorites);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des coups de cœur actifs.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        #endregion
    }
}
