using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Repository.EntityFramwork;

namespace CinephoriaServer.API.Repository
{
    public interface IUnitOfWorkMongoDb : IDisposable
    {
        
        IMongoRepository<AdminDashboard> AdminDashboards { get; }

        Task SaveChangesAsync();
        Task<bool> ExistsAsync<T>(string id) where T : class;
    }
}
