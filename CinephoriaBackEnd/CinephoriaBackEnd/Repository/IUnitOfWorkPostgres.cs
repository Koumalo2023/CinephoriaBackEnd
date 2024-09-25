using CinephoriaBackEnd.Models;
using CinephoriaBackEnd.Repository.EntityFramwork;

namespace CinephoriaBackEnd.Repository
{
    public interface IUnitOfWorkPostgres : IDisposable
    {
        IRepository<Cinema> Cinemas { get; }
        IRepository<Movie> Movies { get; }
        IRepository<Room> Rooms { get; }
        IRepository<Showtime> Showtimes { get; }
        IRepository<Reservation> Reservations { get; }
        IRepository<Review> Reviews { get; }

        Task<int> CompleteAsync();
        int Complete();
    }

}
