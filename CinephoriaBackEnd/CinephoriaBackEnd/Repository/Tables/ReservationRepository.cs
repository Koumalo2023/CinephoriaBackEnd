using CinephoriaBackEnd.Data;
using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore;


namespace CinephoriaBackEnd.Repository
{
    /// <summary>
    /// Interface pour gérer les opérations liées aux réservations
    /// </summary>
    public interface IReservationRepository
    {
        /// <summary>
        /// Récupérer toutes les réservations (pour les administrateurs)
        /// </summary>
        /// <returns>Liste de toutes les réservations</returns>
        Task<List<Reservation>> GetAllReservations(); // GET /api/reservations

        /// <summary>
        /// Créer une nouvelle réservation
        /// </summary>
        /// <param name="reservation">Les détails de la nouvelle réservation</param>
        /// <returns>La réservation créée</returns>
        Task<Reservation> CreateReservation(Reservation reservation); // POST /api/reservations

        /// <summary>
        /// Récupérer les détails d'une réservation spécifique par son ID
        /// </summary>
        /// <param name="id">L'ID de la réservation</param>
        /// <returns>Les détails de la réservation</returns>
        Task<Reservation> GetReservationById(int id); // GET /api/reservations/{id}

        /// <summary>
        /// Modifier une réservation existante
        /// </summary>
        /// <param name="reservation">Les détails mis à jour de la réservation</param>
        Task UpdateReservation(Reservation reservation); // PUT /api/reservations/{id}

        /// <summary>
        /// Supprimer une réservation par son ID
        /// </summary>
        /// <param name="id">L'ID de la réservation</param>
        Task DeleteReservation(int id); // DELETE /api/reservations/{id}

        /// <summary>
        /// Récupérer toutes les réservations effectuées par un utilisateur spécifique
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur</param>
        /// <returns>Liste des réservations de l'utilisateur</returns>
        Task<List<Reservation>> GetUserReservations(string userId); // GET /api/reservations/user/{userId}

        /// <summary>
        /// Récupérer le QR code d'une réservation pour le billet
        /// </summary>
        /// <param name="id">L'ID de la réservation</param>
        /// <returns>Un QR code sous forme de chaîne de caractères ou d'image</returns>
        Task<string> GetReservationQRCode(int id); // GET /api/reservations/{id}/qr
    }

    public class ReservationRepository : IReservationRepository
    {
        private readonly CinephoriaDbContext _context;

        public ReservationRepository(CinephoriaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetAllReservations()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation> CreateReservation(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> GetReservationById(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task UpdateReservation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Reservation>> GetUserReservations(string userId)
        {
            return await _context.Reservations.Where(r => r.AppUserId == userId).ToListAsync();
        }

        public async Task<string> GetReservationQRCode(int id)
        {
            // Implémentation pour générer et retourner un QR code basé sur l'ID de réservation.
            // Cela pourrait inclure l'utilisation d'une librairie pour générer un QR code.
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return null;
            }

            // Logique pour générer un QR code à partir des détails de la réservation.
            return GenerateQRCode(reservation);
        }

        private string GenerateQRCode(Reservation reservation)
        {
            // Utiliser une librairie pour générer le QR code en tant que chaîne de caractères.
            // Exemple fictif:
            return "QRCodeImageBase64";
        }
    }

}
