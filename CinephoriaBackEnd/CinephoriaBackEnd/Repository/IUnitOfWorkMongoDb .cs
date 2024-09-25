using CinephoriaBackEnd.Models;
using CinephoriaBackEnd.Repository.EntityFramwork;

namespace CinephoriaBackEnd.Repository
{
    public interface IUnitOfWorkMongoDb : IDisposable
    {
        Task<List<Incident>> GetAllAsync();
        Task<Incident?> GetByIdAsync(string id);
        Task CreateAsync(Incident incident);
        Task UpdateAsync(Incident incident);
        Task DeleteAsync(string id);
        Task SaveChangesAsync(); // Si vous avez une logique à enregistrer, sinon peut être ignorée.
    }


}
