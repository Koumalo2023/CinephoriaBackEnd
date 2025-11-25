using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeService> _logger;

        public ShowtimeService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<ShowtimeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto)
        {
            if (createShowtimeDto == null)
            {
                throw new ApiException("Les données de la séance sont invalides.", StatusCodes.Status400BadRequest);
            }

            // Calculer le prix de base en fonction de la qualité de projection
            var basePrice = CalculateBasePrice(createShowtimeDto.Quality);

            // Appliquer les ajustements de prix
            var finalPrice = basePrice + createShowtimeDto.PriceAdjustment;

            // Appliquer une promotion si nécessaire
            if (createShowtimeDto.IsPromotion)
            {
                finalPrice *= 0.9m;
            }

            // Mapper le DTO vers l'entité Showtime
            var showtime = _mapper.Map<Showtime>(createShowtimeDto);

            // Assigner le prix calculé à l'objet Showtime
            showtime.Price = finalPrice;

            // Enregistrer la séance dans la base de données
            await _unitOfWork.Showtimes.CreateSessionAsync(showtime);

            _logger.LogInformation("Séance créée avec succès pour le film avec l'ID {MovieId}.", createShowtimeDto.MovieId);
            return "Séance créée avec succès.";
        }

        public async Task<string> UpdateShowtimeAsync(UpdateShowtimeDto updateShowtimeDto)
        {
            // Récupérer la séance existante
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(updateShowtimeDto.ShowtimeId);
            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            // Vérifier si la qualité de projection ou l'heure de début a changé
            bool needsPriceRecalculation = showtime.Quality != updateShowtimeDto.Quality ||
                                           showtime.StartTime != updateShowtimeDto.StartTime;

            // Mapper les données du DTO vers l'entité Showtime
            _mapper.Map(updateShowtimeDto, showtime);

            // Recalculer le prix si nécessaire
            if (needsPriceRecalculation || updateShowtimeDto.PriceAdjustment != 0 || updateShowtimeDto.IsPromotion)
            {
                showtime.Price = CalculateFinalPrice(updateShowtimeDto.Quality, updateShowtimeDto.PriceAdjustment, updateShowtimeDto.IsPromotion);
            }

            showtime.UpdatedAt = DateTime.UtcNow;

            // Enregistrer les modifications dans la base de données
            await _unitOfWork.Showtimes.UpdateSessionAsync(showtime);

            _logger.LogInformation("Séance avec l'ID {ShowtimeId} mise à jour avec succès.", updateShowtimeDto.ShowtimeId);
            return "Séance mise à jour avec succès.";
        }

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> DeleteShowtimeAsync(int showtimeId)
        {
            if (showtimeId <= 0)
            {
                throw new ApiException("L'identifiant de la séance doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            await _unitOfWork.Showtimes.DeleteSessionAsync(showtimeId);

            _logger.LogInformation("Séance avec l'ID {ShowtimeId} supprimée avec succès.", showtimeId);
            return "Séance supprimée avec succès.";
        }

        /// <summary>
        /// Récupère la liste de toutes les séances.
        /// </summary>
        /// <returns>Une liste de séances sous forme de DTO.</returns>
        public async Task<List<ShowtimeDto>> GetAllShowtimesAsync()
        {

            // Récupérer les séances
            var showtimes = await _unitOfWork.Showtimes.GetAllShowtimesAsync();

            // Vérifier que les séances ne sont pas null
            if (showtimes == null)
            {
                _logger.LogWarning("Aucune séance trouvée.");
                return new List<ShowtimeDto>(); // Retourner une liste vide au lieu de null
            }

            // Mapper les séances vers des DTO
            var showtimeDtos = _mapper.Map<List<ShowtimeDto>>(showtimes);

            // Vérifier que le mapping a réussi
            if (showtimeDtos == null)
            {
                _logger.LogError("Erreur lors du mapping des séances.");
                throw new Exception("Erreur lors du mapping des séances.");
            }

            _logger.LogInformation("{Count} séances récupérées.", showtimeDtos.Count);
            return showtimeDtos;
        }

        /// <summary>
        /// Récupère les détails d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Les détails de la séance sous forme de DTO.</returns>
        public async Task<ShowtimeDto> GetShowtimeDetailsAsync(int showtimeId)
        {
            if (showtimeId <= 0)
            {
                throw new ApiException("L'identifiant de la séance doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            var showtimeDto = _mapper.Map<ShowtimeDto>(showtime);
            return showtimeDto;
        }


        private decimal CalculateBasePrice(ProjectionQuality quality)
        {
            return quality switch
            {
                ProjectionQuality.FourDX => 8.00m,
                ProjectionQuality.ThreeD => 6.50m,
                ProjectionQuality.IMAX => 9.00m,
                ProjectionQuality.FourK => 5.00m,
                ProjectionQuality.Standard2D => 4.00m,
                ProjectionQuality.DolbyCinema => 3.00m,
                _ => throw new ArgumentException("Qualité de projection non reconnue.")
            };


        }

        private decimal CalculateFinalPrice(ProjectionQuality quality, decimal priceAdjustment, bool isPromotion)
        {
            var basePrice = CalculateBasePrice(quality);
            var finalPrice = basePrice + priceAdjustment;

            if (isPromotion)
            {
                finalPrice *= 0.9m;
            }

            if (finalPrice <= 0)
            {
                throw new ApiException("Le prix de la séance doit être positif.", StatusCodes.Status400BadRequest);
            }

            return finalPrice;
        }
    }
}
