using System.Linq.Expressions;

namespace CinephoriaServer.API.Repository.EntityFramwork
{
    public interface IReadRepository<TEntity> where TEntity : class
    {
        // Basic queries
        List<TEntity> GetAll(bool trackChanges = false);
        Task<List<TEntity>> GetAllAsync(bool trackChanges = false);
        TEntity? GetById(int id, bool trackChanges = false);
        Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false);

        // Filtered queries
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);

        // Other utilities
        int Count();
        Task<int> CountAsync();
    }
}
