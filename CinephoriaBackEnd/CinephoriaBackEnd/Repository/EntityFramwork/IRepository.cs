namespace CinephoriaBackEnd.Repository.EntityFramwork
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Basic queries
        List<TEntity> GetAll();
        Task<List<TEntity>> GetAllAsync();
        TEntity? GetById(int id);
        Task<TEntity?> GetByIdAsync(int id);

        // Basic commands
        void Create(TEntity entityToCreate);
        Task CreateAsync(TEntity entityToCreate);
        void Update(TEntity entityToUpdate);
        Task UpdateAsync(TEntity entityToUpdate);
        void Delete(TEntity entityToDelete);
        Task DeleteAsync(TEntity entityToDelete);
    }
}
