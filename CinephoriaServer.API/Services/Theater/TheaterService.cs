using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository; 
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TheaterService> _logger;

        public TheaterService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<TheaterService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Récupère la liste des salles de cinéma associées à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de salles sous forme de DTO.</returns>
        public async Task<List<TheaterDto>> GetTheatersByCinemaAsync(int cinemaId)
        {
            var theaters = await _unitOfWork.Theaters.GetTheatersByCinemaAsync(cinemaId);
            var theaterDtos = _mapper.Map<List<TheaterDto>>(theaters);

            _logger.LogInformation("{Count} salles récupérées pour le cinéma avec l'ID {CinemaId}.", theaterDtos.Count, cinemaId);
            return theaterDtos;
        }

        /// <summary>
        /// Récupère la liste de tous les théâtres.
        /// </summary>
        /// <returns>Une liste de tous les théâtres sous forme de DTO.</returns>
        public async Task<List<TheaterDto>> GetAllTheatersAsync()
        {
            var theaters = await _unitOfWork.Theaters.GetAllTheatersAsync();
            var theaterDtos = _mapper.Map<List<TheaterDto>>(theaters);

            _logger.LogInformation("{Count} salles récupérées au total.", theaterDtos.Count);
            return theaterDtos;
        }

        /// <summary>
        /// Récupère une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>La salle correspondante sous forme de DTO.</returns>
        public async Task<TheaterDto> GetTheaterByIdAsync(int theaterId)
        {
            var theater = await _unitOfWork.Theaters.GetTheaterByIdAsync(theaterId);
            if (theater == null)
            {
                _logger.LogWarning("Salle avec l'ID {TheaterId} non trouvée.", theaterId);
                throw new ApiException("Salle non trouvée.", StatusCodes.Status404NotFound);
            }

            var theaterDto = _mapper.Map<TheaterDto>(theater);
            _logger.LogInformation("Salle avec l'ID {TheaterId} récupérée avec succès.", theaterId);
            return theaterDto;
        }

        /// <summary>
        /// Crée une nouvelle salle de cinéma.
        /// </summary>
        /// <param name="createTheaterDto">Les données de la salle à créer.</param>
        /// <returns>La salle créée sous forme de DTO.</returns>
        public async Task<string> CreateTheaterAsync(CreateTheaterDto createTheaterDto)
        {
            // Valider le DTO en utilisant les annotations de données
            var validationContext = new ValidationContext(createTheaterDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(createTheaterDto, validationContext, validationResults, true);

            if (!isValid)
            {
                // Si la validation échoue, lever une exception
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                _logger.LogWarning("Échec de la validation lors de la création d'une salle : {Errors}", string.Join(", ", errors));
                throw new ApiException("Erreurs de validation.", StatusCodes.Status400BadRequest);
            }

            // Mapping et création de la salle
            var theater = _mapper.Map<Theater>(createTheaterDto);
            await _unitOfWork.Theaters.CreateTheaterAsync(theater);
            await _unitOfWork.CompleteAsync();

            // Créer les sièges pour la salle
            await CreateSeatsForTheaterAsync(theater.TheaterId, createTheaterDto.SeatCount, theater.Name);

            var theaterDto = _mapper.Map<TheaterDto>(theater);
            _logger.LogInformation("Salle créée avec succès avec l'ID {TheaterId}.", theater.TheaterId);
            return "Salle créée avec succès.";
        }

        /// <summary>
        /// Met à jour les informations d'une salle de cinéma existante.
        /// </summary>
        /// <param name="updateTheaterDto">Les données de la salle à mettre à jour.</param>
        /// <returns>La salle mise à jour sous forme de DTO.</returns>
        public async Task<string> UpdateTheaterAsync(UpdateTheaterDto updateTheaterDto)
        {
            // Récupérer la salle avec ses sièges
            var theater = await _unitOfWork.Theaters.GetTheaterByIdAsync(updateTheaterDto.TheaterId);
            if (theater == null)
            {
                _logger.LogWarning("Salle avec l'ID {TheaterId} non trouvée pour la mise à jour.", updateTheaterDto.TheaterId);
                throw new ApiException("Salle non trouvée.", StatusCodes.Status404NotFound);
            }

            // Vérifier si le nombre de sièges a changé
            bool seatCountChanged = updateTheaterDto.SeatCount != theater.SeatCount;

            // Vérifier si le nom de la salle a changé
            bool nameChanged = updateTheaterDto.Name != theater.Name;

            // Appliquer les modifications
            _mapper.Map(updateTheaterDto, theater);
            theater.UpdatedAt = DateTime.UtcNow;

            // Si le nombre de sièges ou le nom de la salle a changé, mettre à jour les sièges
            if (seatCountChanged || nameChanged)
            {
                // Supprimer les sièges existants
                await _unitOfWork.Seats.DeleteSeatsByTheaterIdAsync(theater.TheaterId);

                // Recréer les sièges avec le nouveau nombre et le nouveau nom
                await CreateSeatsForTheaterAsync(theater.TheaterId, updateTheaterDto.SeatCount, theater.Name);
            }

            await _unitOfWork.Theaters.UpdateTheaterAsync(theater);
            await _unitOfWork.CompleteAsync();

            // Mapper l'entité Theater vers TheaterDto
            var theaterDto = _mapper.Map<TheaterDto>(theater);
            _logger.LogInformation("Salle avec l'ID {TheaterId} mise à jour avec succès.", theater.TheaterId);
            return "Salle mise à jour avec succès.";
        }

        /// <summary>
        /// Supprime une salle de cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<bool> DeleteTheaterAsync(int theaterId)
        {
            // Récupérer la salle avec ses sièges
            var theater = await _unitOfWork.Theaters.GetTheaterByIdAsync(theaterId);
            if (theater == null)
            {
                _logger.LogWarning("Salle avec l'ID {TheaterId} non trouvée pour la suppression.", theaterId);
                throw new ApiException("Salle non trouvée.", StatusCodes.Status404NotFound);
            }
            // Logique de suppression
            await _unitOfWork.Theaters.DeleteTheaterAsync(theaterId);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        public async Task<List<IncidentDto>> GetTheaterIncidentsAsync(int theaterId)
        {
            if (theaterId <= 0)
            {
                _logger.LogWarning("L'ID de la salle doit être un nombre positif.");
                throw new ApiException("L'ID de la salle doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            // Récupérer les incidents associés à la salle
            var incidents = await _unitOfWork.Theaters.GetTheaterIncidentsAsync(theaterId);
            var incidentDtos = _mapper.Map<List<IncidentDto>>(incidents);

            _logger.LogInformation("{Count} incidents récupérés pour la salle avec l'ID {TheaterId}.", incidentDtos.Count, theaterId);
            return incidentDtos;
        }

        /// <summary>
        /// Crée les sièges pour une salle spécifique.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <param name="seatCount">Le nombre de sièges à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        private async Task<string> CreateSeatsForTheaterAsync(int theaterId, int seatCount, string theaterName)
        {
            var seats = new List<Seat>();

            // Extraire la première lettre du nom de la salle (par exemple, "A" pour "Salle A")
            var prefix = theaterName.Split(' ').Last(); // "Salle A" -> "A"

            for (int i = 1; i <= seatCount; i++)
            {
                var seat = new Seat
                {
                    TheaterId = theaterId,
                    SeatNumber = $"{prefix}{i}", // Exemple : "A1", "A2", etc.
                    IsAccessible = true,        // Par défaut, le siège n'est pas accessible
                    IsAvailable = true           // Par défaut, le siège est disponible
                };
                seats.Add(seat);
            }

            await _unitOfWork.Seats.AddSeatsAsync(seats);
            await _unitOfWork.CompleteAsync();
            

            _logger.LogInformation("{Count} sièges créés pour la salle avec l'ID {TheaterId}.", seatCount, theaterId);
            return "Sièges créés succès";
        }
    }
}
