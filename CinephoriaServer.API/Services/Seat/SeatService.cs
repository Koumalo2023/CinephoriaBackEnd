using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using Microsoft.Extensions.Logging;

namespace CinephoriaServer.API.Services
{
    public class SeatService : ISeatService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SeatService> _logger;

        public SeatService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<SeatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles sous forme de DTO.</returns>
        public async Task<List<SeatDto>> GetAvailableSeatsAsync(int sessionId)
        {
            var seats = await _unitOfWork.Seats.GetAvailableSeatsAsync(sessionId);
            var seatDtos = _mapper.Map<List<SeatDto>>(seats);

            _logger.LogInformation("{Count} sièges disponibles récupérés pour la séance avec l'ID {SessionId}.", seatDtos.Count, sessionId);
            return seatDtos;
        }


        /// <inheritdoc />
        public async Task<IEnumerable<SeatDto>> GetSeatsByTheaterIdAsync(int theaterId)
        {
            var seats = await _unitOfWork.Seats.GetSeatsByTheaterIdAsync(theaterId);
            return _mapper.Map<IEnumerable<SeatDto>>(seats);
        }

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<bool> AddHandicapSeatAsync(int theaterId, string seatNumber)
        {
            await _unitOfWork.Seats.AddHandicapSeatAsync(theaterId, seatNumber);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Siège réservé pour personnes à mobilité réduite ajouté avec succès dans la salle avec l'ID {TheaterId}.", theaterId);
            return true;
        }

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<bool> RemoveHandicapSeatAsync(int theaterId, string seatNumber)
        {
            await _unitOfWork.Seats.RemoveHandicapSeatAsync(theaterId, seatNumber);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Siège réservé pour personnes à mobilité réduite supprimé avec succès dans la salle avec l'ID {TheaterId}.", theaterId);
            return true;
        }

        // <summary>
        /// Met à jour un siège existant.
        /// </summary>
        /// <param name="updateSeatDto">Les nouvelles informations du siège.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        public async Task<string> UpdateSeatAsync(UpdateSeatDto updateSeatDto)
        {
            // Récupérer le siège existant
            var existingSeat = await _unitOfWork.Seats.GetByIdAsync(updateSeatDto.SeatId);
            if (existingSeat == null)
            {
                throw new ApiException("Siège non trouvé.", StatusCodes.Status404NotFound);
            }

            // Mapper les nouvelles valeurs sur l'entité existante
            _mapper.Map(updateSeatDto, existingSeat);
            existingSeat.UpdatedAt = DateTime.UtcNow;

            // Enregistrer les modifications dans la base de données
            await _unitOfWork.Seats.UpdateSeatAsync(existingSeat);

            _logger.LogInformation("Siège avec l'ID {SeatId} mis à jour avec succès.", updateSeatDto.SeatId);
            return "Siège mis à jour avec succès.";
        }


        


    }
}
