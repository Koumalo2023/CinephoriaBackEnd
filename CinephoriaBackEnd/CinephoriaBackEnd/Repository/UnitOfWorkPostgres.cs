using CinephoriaBackEnd.Data;
using CinephoriaBackEnd.Models;
using CinephoriaBackEnd.Repository.EntityFramwork;

namespace CinephoriaBackEnd.Repository
{
    public class UnitOfWorkPostgres : IUnitOfWorkPostgres
    {
        private readonly CinephoriaDbContext _context;

        // Déclaration des repositories spécifiques
        public IRepository<Cinema> Cinemas { get; private set; }
        public IRepository<Movie> Movies { get; private set; }
        public IRepository<Room> Rooms { get; private set; }
        public IRepository<Showtime> Showtimes { get; private set; }
        public IRepository<Reservation> Reservations { get; private set; }
        public IRepository<Review> Reviews { get; private set; }

        // Constructeur qui injecte le contexte et initialise les repositories
        public UnitOfWorkPostgres(CinephoriaDbContext context)
        {
            _context = context;
            Cinemas = new EFRepository<Cinema>(context);
            Movies = new EFRepository<Movie>(context);
            Rooms = new EFRepository<Room>(context);
            Showtimes = new EFRepository<Showtime>(context);
            Reservations = new EFRepository<Reservation>(context);
            Reviews = new EFRepository<Review>(context);
        }

        // Méthode pour sauvegarder les modifications dans la base de données
        public int Complete()
        {
            return _context.SaveChanges();
        }

        // Méthode asynchrone pour sauvegarder les modifications
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose du contexte
        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
