
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface IReservationService
    {
        /// <summary>
        /// Récupère la liste de toutes les réservations d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<ReservationDto>> GetReservationsByShowtimeAsync(int showtimeId);

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances disponibles.</returns>
        Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<SeatDto>> GetAvailableSeatsAsync(int showtimeId);

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations sous forme de DTO.</returns>
        Task<List<UserReservationDto>> GetUserReservationsAsync(string AppUserId);

        /// <summary>
        /// Récupère une réservation spécifique par son identifiant
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation</param>
        /// <returns>Les détails de la réservation</returns>
        Task<ReservationDto?> GetReservationByIdAsync(int reservationId);

        /// <summary>
        /// Crée une nouvelle réservation.
        /// </summary>
        /// <param name="createReservationDto">Les données de la réservation à créer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        Task<string> CreateReservationAsync(CreateReservationDto createReservationDto);

        /// <summary>
        /// Annule une réservation existante.
        /// </summary>
        Task<string> CancelReservationAsync(int reservationId);


        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <param name="seatNumbers">La liste des numéros de sièges sélectionnés.</param>
        /// <returns>Le prix total de la réservation.</returns>
        Task<decimal> CalculateReservationPriceAsync(int showtimeId, List<string> seatNumbers);

        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        Task HoldSeatsAsync(int showtimeId, List<string> seatNumbers);


        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>True si la validation est réussie, sinon False.</returns>
        Task<bool> ValidatedSession(string qrCodeData);
    }

}
