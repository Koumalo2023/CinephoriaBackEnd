using CinephoriaServer.API.Data;
using CinephoriaServer.API.Repository.EntityFramwork;
using CinephoriaServer.API.Services;

namespace CinephoriaServer.API.Repository
{
    public class UnitOfWorkPostgres : IUnitOfWorkPostgres
    {
        private readonly CinephoriaDbContext _context;
        private readonly QRCodeService _qrCodeService;
        private readonly Dictionary<Type, object> _readRepositories = new();
        private readonly Dictionary<Type, object> _writeRepositories = new();

        // Repositories spécifiques
        private ICinemaRepository _cinemas; 
        private IMovieRepository _movies;
        private IMovieRatingRepository _movieRatings;
        private ITheaterRepository _theaters;
        private ISeatRepository _seats;
        private IShowtimeRepository _showtimes;
        private IReservationRepository _reservations;
        private IIncidentRepository _incidents;
        private IUserRepository _users;
        private IUserMovieHistoryRepository _userMovieHistories;

        public UnitOfWorkPostgres(CinephoriaDbContext context, QRCodeService qrCodeService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
        }

        // Propriétés pour accéder aux repositories spécifiques
        public ICinemaRepository Cinemas => _cinemas ??= new CinemaRepository(_context);
        public IMovieRepository Movies => _movies ??= new MovieRepository(_context);
        public IMovieRatingRepository MovieRatings => _movieRatings ??= new MovieRatingRepository(_context);
        public ITheaterRepository Theaters => _theaters ??= new TheaterRepository(_context);
        public ISeatRepository Seats => _seats ??= new SeatRepository(_context);
        public IShowtimeRepository Showtimes => _showtimes ??= new ShowtimeRepository(_context);
        public IReservationRepository Reservations => _reservations ??= new ReservationRepository(_context, _qrCodeService);
        public IIncidentRepository Incidents => _incidents ??= new IncidentRepository(_context);
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IUserMovieHistoryRepository UserMovieHistories => _userMovieHistories ??= new UserMovieHistoryRepository(_context);

        // Méthodes génériques pour les repositories
        public IReadRepository<TEntity> ReadRepository<TEntity>() where TEntity : class
        {
            if (_readRepositories.ContainsKey(typeof(TEntity)))
            {
                return (IReadRepository<TEntity>)_readRepositories[typeof(TEntity)];
            }

            var repository = new EFRepository<TEntity>(_context);
            _readRepositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public IWriteRepository<TEntity> WriteRepository<TEntity>() where TEntity : class
        {
            if (_writeRepositories.ContainsKey(typeof(TEntity)))
            {
                return (IWriteRepository<TEntity>)_writeRepositories[typeof(TEntity)];
            }

            var repository = new EFRepository<TEntity>(_context);
            _writeRepositories.Add(typeof(TEntity), repository);
            return repository;
        }

        // Méthodes pour sauvegarder les changements
        public int Complete()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while saving changes to the database.", ex);
            }
        }

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while saving changes to the database.", ex);
            }
        }

        // Dispose du contexte
        public async Task<int> SaveAsync()
        {
            return await CompleteAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
