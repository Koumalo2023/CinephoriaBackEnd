    using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CinemaService> _logger;

        public CinemaService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<CinemaService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Récupère la liste de tous les cinémas.
        /// </summary>
        /// <returns>Une liste de cinémas sous forme de DTO.</returns>
        public async Task<List<CinemaDto>> GetAllCinemasAsync()
        {
            var cinemas = await _unitOfWork.Cinemas.GetAllCinemasAsync();
            var cinemaDtos = _mapper.Map<List<CinemaDto>>(cinemas);

            _logger.LogInformation("{Count} cinémas récupérés avec succès.", cinemaDtos.Count);
            return cinemaDtos;
        }

        /// <summary>
        /// Récupère un cinéma par son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Le cinéma correspondant sous forme de DTO.</returns>
        public async Task<CinemaDto> GetCinemaByIdAsync(int cinemaId)
        {
            var cinema = await _unitOfWork.Cinemas.GetCinemaByIdAsync(cinemaId);
            if (cinema == null)
            {
                _logger.LogWarning("Cinéma avec l'ID {CinemaId} non trouvé.", cinemaId);
                throw new ApiException("Cinéma non trouvé.", StatusCodes.Status404NotFound);
            }

            _logger.LogInformation("Cinéma avec l'ID {CinemaId} récupéré avec succès.", cinemaId);
            return _mapper.Map<CinemaDto>(cinema);
        }

        /// <summary>
        /// Crée un nouveau cinéma.
        /// </summary>
        /// <param name="createCinemaDto">Les données du cinéma à créer.</param>
        /// <returns>Le cinéma créé sous forme de DTO.</returns>
        public async Task<string> CreateCinemaAsync(CreateCinemaDto createCinemaDto)
        {
            // Valider le DTO en utilisant les annotations de données
            var validationContext = new ValidationContext(createCinemaDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(createCinemaDto, validationContext, validationResults, true);

            if (!isValid)
            {
                // Si la validation échoue, lever une exception
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                _logger.LogWarning("Échec de la validation lors de la création d'un cinéma : {Errors}", string.Join(", ", errors));
                throw new ApiException("Erreurs de validation.", StatusCodes.Status400BadRequest);
            }

            // Mapping et création du cinéma
            var cinema = _mapper.Map<Cinema>(createCinemaDto);
            await _unitOfWork.Cinemas.CreateCinemaAsync(cinema);
            await _unitOfWork.CompleteAsync();

            var cinemaDto = _mapper.Map<CinemaDto>(cinema);
            _logger.LogInformation("Cinéma créé avec succès avec l'ID {CinemaId}.", cinema.CinemaId);
            return "Cinéma créé avec succès";
        }

        /// <summary>
        /// Met à jour les informations d'un cinéma existant.
        /// </summary>
        /// <param name="updateCinemaDto">Les données du cinéma à mettre à jour.</param>
        /// <returns>Le cinéma mis à jour sous forme de DTO.</returns>
        public async Task<string> UpdateCinemaAsync(UpdateCinemaDto updateCinemaDto)
        {
            var cinema = await _unitOfWork.Cinemas.GetCinemaByIdAsync(updateCinemaDto.CinemaId);
            if (cinema == null)
            {
                _logger.LogWarning("Cinéma avec l'ID {CinemaId} non trouvé pour la mise à jour.", updateCinemaDto.CinemaId);
                throw new ApiException("Cinéma non trouvé.", StatusCodes.Status404NotFound);
            }

            _mapper.Map(updateCinemaDto, cinema);
            cinema.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Cinemas.UpdateCinemaAsync(cinema);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Cinéma avec l'ID {CinemaId} mis à jour avec succès.", cinema.CinemaId);
            return "Cinéma avec l'ID {CinemaId} mis à jour avec succès." ;
        }

        /// <summary>
        /// Supprime un cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<string> DeleteCinemaAsync(int cinemaId)
        {
            var cinema = await _unitOfWork.Cinemas.GetCinemaByIdAsync(cinemaId);
            if (cinema == null)
            {
                _logger.LogWarning("Cinéma avec l'ID {CinemaId} non trouvé pour la suppression.", cinemaId);
                throw new ApiException("Cinéma non trouvé.", StatusCodes.Status404NotFound);
            }

            await _unitOfWork.Cinemas.DeleteCinemaAsync(cinemaId);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Cinéma avec l'ID {CinemaId} supprimé avec succès.", cinemaId);
            return "Cinéma supprimé avec succès.";
        }
    }

}
