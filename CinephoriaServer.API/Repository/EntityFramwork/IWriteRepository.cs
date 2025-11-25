namespace CinephoriaServer.API.Repository.EntityFramwork
{
    public interface IWriteRepository<TEntity> where TEntity : class
    {
        // Basic commands
        void Create(TEntity entity);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        void Delete(TEntity entity);
    }
}
