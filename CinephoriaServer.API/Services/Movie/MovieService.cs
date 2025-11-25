using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models;
using CinephoriaServer.API.Repository;  

namespace CinephoriaServer.API.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;
        private readonly IImageService _imageService;
        private readonly ITMDbService _tmdbService;
        private readonly TMDbMovieMapper _tmdbMovieMapper;

        public MovieService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<MovieService> logger,
                          IImageService imageService, ITMDbService tmdbService, TMDbMovieMapper tmdbMovieMapper)
        {
             _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
            _tmdbService = tmdbService;
            _tmdbMovieMapper = tmdbMovieMapper;
        }

        /// <summary>
        /// Récupère la liste des derniers films ajoutés avec au moins une séance.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        public async Task<List<MovieDto>> GetRecentMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetRecentMoviesAsync();
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Récupération des derniers films réussie.");
            return movieDtos;
        }

        /// <summary>
        /// Récupère la liste de tous les films.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        public async Task<List<MovieDto>> GetAllMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllMoviesAsync();
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Récupération de tous les films réussie.");
            return movieDtos;
        }


        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        public async Task<MovieDetailsDto> GetMovieDetailsAsync(int movieId)
        {
            var movie = await _unitOfWork.Movies.GetMovieDetailsAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            var movieDetailsDto = _mapper.Map<MovieDetailsDto>(movie);

            _logger.LogInformation("Détails du film avec l'ID {MovieId} récupérés avec succès.", movieId);
            return movieDetailsDto;
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        public async Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId)
        {
            var showtimes = await _unitOfWork.Movies.GetMovieSessionsAsync(movieId);
            var showtimeDtos = _mapper.Map<List<ShowtimeDto>>(showtimes);

            _logger.LogInformation("Séances du film avec l'ID {MovieId} récupérées avec succès.", movieId);
            return showtimeDtos;
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="reviewDto">Les données de l'avis.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> SubmitMovieReviewAsync(MovieReviewDto reviewDto)
        {
            await _unitOfWork.Movies.SubmitMovieReviewAsync(reviewDto);

            _logger.LogInformation("Avis soumis avec succès pour le film avec l'ID {MovieId}.", reviewDto.MovieId);
            return "Avis soumis avec succès";
        }

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre, de la date, de l'année, du réalisateur, des acteurs et de la classification.
        /// </summary>
        /// <param name="filterDto">Les critères de filtrage.</param>
        /// <returns>Une liste de films correspondant aux critères.</returns>
        public async Task<List<MovieDto>> FilterMoviesAsync(FilterMoviesRequestDto filterDto)
        {
            var movies = await _unitOfWork.Movies.FilterMoviesAsync(
                filterDto.CinemaId,
                filterDto.Genre,
                filterDto.Date,
                filterDto.Year,
                filterDto.Director,
                filterDto.Actor,
                filterDto.MinimumAge
            );
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Filtrage des films réussi.");
            return movieDtos;
        }

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="createMovieDto">Les données du film à créer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<int> CreateMovieAsync(CreateMovieDto createMovieDto)
        {
            var movie = _mapper.Map<Movie>(createMovieDto);
            await _unitOfWork.Movies.CreateMovieAsync(movie);

            _logger.LogInformation("Film créé avec succès.");
            return movie.MovieId;
        }

        /// <summary>
        /// Ajoute une affiche à un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> AddPosterToMovieAsync(int movieId, string posterUrl)
        {
            // Récupérer le film existant
            var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            // Ajouter l'URL de l'affiche à la liste des affiches du film
            movie.PosterUrls = posterUrl;

            // Mettre à jour le film dans la base de données
            await _unitOfWork.Movies.UpdateAsync(movie);

            _logger.LogInformation("Affiche ajoutée avec succès au film avec l'ID {MovieId}.", movieId);
            return "Affiche ajoutée avec succès au film";
        }

        /// <summary>
        /// Supprime une affiche d'un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> RemovePosterFromMovieAsync(int movieId, string posterUrl)
        {
            // Récupérer le film existant
            var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            // Supprimer l'affiche du stockage
            var imageDeleted = await _imageService.DeleteImageAsync(posterUrl);
            if (!imageDeleted)
            {
                throw new ApiException("L'affiche n'a pas pu être supprimée du stockage.", StatusCodes.Status500InternalServerError);
            }

            // Supprimer l'URL de l'affiche de la liste des affiches du film
            movie.PosterUrls = posterUrl;

            // Mettre à jour le film dans la base de données
            await _unitOfWork.Movies.UpdateAsync(movie);

            _logger.LogInformation("Affiche supprimée avec succès du film avec l'ID {MovieId}.", movieId);
            return "Affiche supprimée avec succès.";
        }

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="updateMovieDto">Les nouvelles données du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateMovieAsync(UpdateMovieDto updateMovieDto)
        {
            var movie = _mapper.Map<Movie>(updateMovieDto);
            await _unitOfWork.Movies.UpdateMovieAsync(movie);

            _logger.LogInformation("Film avec l'ID {MovieId} mis à jour avec succès.", updateMovieDto.MovieId);
            return "Film  mis à jour avec succès.";
        }

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> DeleteMovieAsync(int movieId)
        {
            await _unitOfWork.Movies.DeleteMovieAsync(movieId);

            _logger.LogInformation("Film avec l'ID {MovieId} supprimé avec succès.", movieId);
            return "Film supprimé avec succès.";
        }

        /// <summary>
        /// Récupère la liste des films qui ont des séances dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        public async Task<List<MovieDto>> GetMoviesByCinemaIdAsync(int cinemaId)
        {
            var movies = await _unitOfWork.Movies.GetMoviesByCinemaIdAsync(cinemaId);
            if (movies == null || !movies.Any())
            {
                _logger.LogWarning("Aucun film trouvé pour le cinéma avec l'ID {CinemaId}.", cinemaId);
                throw new ApiException("Aucun film trouvé pour ce cinéma.", StatusCodes.Status404NotFound);
            }

            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Films récupérés avec succès pour le cinéma avec l'ID {CinemaId}.", cinemaId);
            return movieDtos;
        }

        /// <summary>
        /// Enregistre la consultation d'un film par un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> RecordMovieViewAsync(string userId, int movieId)
        {
            // Vérifier si l'utilisateur existe
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur avec l'ID {UserId} non trouvé.", userId);
                throw new ApiException("Utilisateur non trouvé.", StatusCodes.Status404NotFound);
            }

            // Vérifier si le film existe
            var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            // Enregistrer ou mettre à jour l'historique de consultation
            await _unitOfWork.UserMovieHistories.RecordMovieViewAsync(userId, movieId);

            _logger.LogInformation("Consultation du film {MovieId} par l'utilisateur {UserId} enregistrée avec succès.", movieId, userId);
            return "Consultation enregistrée avec succès";
        }

        /// <summary>
        /// Récupère l'historique des films consultés par un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="limit">Nombre maximum de films à récupérer (optionnel).</param>
        /// <returns>Une liste de films consultés récemment.</returns>
        public async Task<List<MovieDto>> GetUserMovieHistoryAsync(string userId, int? limit = null)
        {
            // Vérifier si l'utilisateur existe
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur avec l'ID {UserId} non trouvé.", userId);
                throw new ApiException("Utilisateur non trouvé.", StatusCodes.Status404NotFound);
            }

            // Récupérer l'historique des films consultés
            var movieHistory = await _unitOfWork.UserMovieHistories.GetUserMovieHistoryAsync(userId, limit);

            // Mapper vers des DTOs de films
            var movieDtos = _mapper.Map<List<MovieDto>>(movieHistory);

            _logger.LogInformation("Historique des films consultés par l'utilisateur {UserId} récupéré avec succès.", userId);
            return movieDtos;
        }

        /// <summary>
        /// Récupère tous les films qui ont au moins une séance programmée.
        /// </summary>
        /// <returns>Une liste de films avec séances.</returns>
        public async Task<List<MovieDto>> GetAllMoviesWithShowtimeAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllMoviesWithShowtimeAsync();
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Récupération de tous les films avec séances programmées réussie.");
            return movieDtos;
        }

        /// <summary>
        /// Recherche des films dans TMDb par titre.
        /// </summary>
        /// <param name="searchRequest">Requête de recherche TMDb.</param>
        /// <returns>Résultats de la recherche TMDb.</returns>
        public async Task<TMDbSearchResult> SearchTMDbMoviesAsync(TMDbSearchRequestDto searchRequest)
        {
            try
            {
                var result = await _tmdbService.SearchMoviesAsync(searchRequest.Query, searchRequest.Page);
                _logger.LogInformation("Recherche TMDb réussie pour : {Query} - {Count} résultats", searchRequest.Query, result.Results?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche TMDb pour : {Query}", searchRequest.Query);
                throw new ApiException("Erreur lors de la recherche TMDb", 500);
            }
        }

        /// <summary>
        /// Importe un film depuis TMDb dans la base locale.
        /// </summary>
        /// <param name="importRequest">Requête d'import TMDb.</param>
        /// <returns>Identifiant du film importé.</returns>
        public async Task<int> ImportMovieFromTMDbAsync(TMDbImportRequestDto importRequest)
        {
            try
            {
                // Vérifier si le film existe déjà dans la base locale
                var existingMovie = await _unitOfWork.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId);
                if (existingMovie != null)
                {
                    _logger.LogWarning("Le film TMDb avec l'ID {TmdbId} existe déjà dans la base locale (MovieId: {MovieId})",
                        importRequest.TmdbId, existingMovie.MovieId);
                    throw new ApiException("Ce film existe déjà dans la base de données", 409);
                }

                // Récupérer les détails du film depuis TMDb
                var tmdbMovie = await _tmdbService.GetMovieDetailsAsync(importRequest.TmdbId);
                
                // Utiliser le mapper pour convertir TMDbMovieDetails en Movie
                var movie = _tmdbMovieMapper.MapToMovie(tmdbMovie);

                // Récupérer les vidéos (bandes-annonces) du film depuis TMDb
                try
                {
                    var videoResponse = await _tmdbService.GetMovieVideosAsync(importRequest.TmdbId);
                    _tmdbMovieMapper.UpdateMovieTrailer(movie, videoResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Impossible de récupérer les bandes-annonces pour le film TMDb ID {TmdbId}", importRequest.TmdbId);
                    // On continue sans la bande-annonce
                }

                // Créer le film dans la base de données
                await _unitOfWork.Movies.CreateMovieAsync(movie);

                _logger.LogInformation("Film importé avec succès depuis TMDb : {Title} (TMDb ID: {TmdbId}, MovieId: {MovieId})",
                    movie.Title, importRequest.TmdbId, movie.MovieId);

                return movie.MovieId;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'import du film TMDb avec l'ID {TmdbId}", importRequest.TmdbId);
                throw new ApiException("Erreur lors de l'import du film", 500);
            }
        }

    }

}
