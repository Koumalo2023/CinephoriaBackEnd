using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.API.Services
{
    public class MovieRatingService : IMovieRatingService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieRatingService> _logger;

        public MovieRatingService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<MovieRatingService> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="createMovieRatingDto">Les données de l'avis à soumettre.</param>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> SubmitMovieReviewAsync(CreateMovieRatingDto createMovieRatingDto, string AppUserId)
        {
            if (createMovieRatingDto.MovieId <= 0 || string.IsNullOrWhiteSpace(AppUserId))
            {
                throw new ApiException("Entrées invalides.", StatusCodes.Status400BadRequest);
            }

            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null) throw new ApiException("Utilisateur introuvable.", StatusCodes.Status400BadRequest);

            var movieExists = await _unitOfWork.Movies.GetByIdAsync(createMovieRatingDto.MovieId) != null;
            if (!movieExists) throw new ApiException("Film introuvable.", StatusCodes.Status400BadRequest);

            var movieRating = _mapper.Map<MovieRating>(createMovieRatingDto);
            movieRating.AppUserId = AppUserId;
            movieRating.IsValidated = false;

            await _unitOfWork.MovieRatings.CreateAsync(movieRating);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis soumis avec succès pour le film avec l'ID {MovieId}.", createMovieRatingDto.MovieId);
            return "Avis soumis avec succès.";

        }

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs et aux employés).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> ValidateReviewAsync(int reviewId)
        {
            if (reviewId <= 0)
            {
                throw new ApiException("L'identifiant de l'avis doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.MovieRatings.GetByIdAsync(reviewId);
            if (review == null) throw new ApiException("Avis introuvable.", StatusCodes.Status404NotFound);

            review.IsValidated = true;
            await _unitOfWork.MovieRatings.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis avec l'ID {ReviewId} validé avec succès.", reviewId);
            return "Avis validé avec succès.";
        }

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> DeleteReviewAsync(int reviewId)
        {
            if (reviewId <= 0)
            {
                throw new ApiException("L'identifiant de l'avis doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.MovieRatings.GetByIdAsync(reviewId);
            if (review == null) throw new ApiException("Avis introuvable.", StatusCodes.Status404NotFound);

            await _unitOfWork.MovieRatings.DeleteReviewAsync(reviewId);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis avec l'ID {ReviewId} supprimé avec succès.", reviewId);
            return "Avis supprimé avec succès.";
        }


        /// <summary>
        /// Récupère la liste des avis associés à un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis sous forme de DTO.</returns>
        public async Task<List<MovieRatingDto>> GetMovieReviewsAsync(int movieId)
        {
            if (movieId <= 0)
            {
                throw new ApiException("L'identifiant du film doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var reviews = await _unitOfWork.MovieRatings.GetMovieReviewsAsync(movieId);
            var reviewDtos = _mapper.Map<List<MovieRatingDto>>(reviews);

            _logger.LogInformation("{Count} avis récupérés pour le film avec l'ID {MovieId}.", reviewDtos.Count, movieId);
            return reviewDtos;
        }

        /// <summary>
        /// Récupère les détails d'un avis spécifique.
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis.</param>
        /// <returns>Les détails de l'avis sous forme de DTO.</returns>
        public async Task<MovieRatingDto> GetReviewDetailsAsync(int reviewId)
        {
            if (reviewId <= 0)
            {
                throw new ApiException("L'identifiant de l'avis doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.MovieRatings.GetByIdAsync(reviewId);
            if (review == null) throw new ApiException("Avis introuvable.", StatusCodes.Status404NotFound);

            var reviewDto = _mapper.Map<MovieRatingDto>(review);
            return reviewDto;
        }

        /// <summary>
        /// Met à jour un avis existant.
        /// </summary>
        /// <param name="updateMovieRatingDto">Les données mises à jour de l'avis.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> UpdateReviewAsync(UpdateMovieRatingDto updateMovieRatingDto)
        {
            if (updateMovieRatingDto.MovieRatingId <= 0)
            {
                throw new ApiException("L'identifiant de l'avis doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.MovieRatings.GetByIdAsync(updateMovieRatingDto.MovieRatingId);
            if (review == null) throw new ApiException("Avis introuvable.", StatusCodes.Status404NotFound);

            _mapper.Map(updateMovieRatingDto, review);
            await _unitOfWork.MovieRatings.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis avec l'ID {ReviewId} mis à jour avec succès.", updateMovieRatingDto.MovieRatingId);
            return "Avis mis à jour avec succès.";
        }
    }
}
