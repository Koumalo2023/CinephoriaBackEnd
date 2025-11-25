using MongoDB.Driver;
using System.Linq.Expressions;

namespace CinephoriaServer.API.Repository.EntityFramwork
{
    public interface IMongoRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<List<T>> FilterAsync(FilterDefinition<T> filter);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        Task UpdatePartialAsync(string id, UpdateDefinition<T> updates);
        Task<T> FindOneAsync(FilterDefinition<T> filter);
        Task<bool> ExistsAsync(FilterDefinition<T> filter);
        Task<List<T>> FilterMovieAsync(FilterDefinition<T> filter,int page = 1, int pageSize = 10,SortDefinition<T> sort = null);
    }

}
