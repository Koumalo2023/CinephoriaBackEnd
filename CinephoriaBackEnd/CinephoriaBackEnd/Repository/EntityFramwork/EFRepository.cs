
using CinephoriaBackEnd.Data;
using CinephoriaBackEnd.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Repository
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly CinephoriaDbContext _efContext;

        public EFRepository(CinephoriaDbContext context)
        {
            _efContext = context;
        }

        // Basic queries

        public List<TEntity> GetAll()
        {
            return _efContext.Set<TEntity>().ToList();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _efContext.Set<TEntity>().ToListAsync();
        }

        public TEntity? GetById(int id)
        {
            return _efContext.Set<TEntity>().Find(id);
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _efContext.Set<TEntity>().FindAsync(id);
        }

        // Basic commands

        public void Create(TEntity entityToCreate)
        {
            _efContext.Set<TEntity>().Add(entityToCreate);
            _efContext.SaveChanges(); 
        }

        public async Task CreateAsync(TEntity entityToCreate)
        {
            await _efContext.Set<TEntity>().AddAsync(entityToCreate);
            await _efContext.SaveChangesAsync(); 
        }

        public void Update(TEntity entityToUpdate)
        {
            _efContext.Set<TEntity>().Update(entityToUpdate);
            _efContext.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entityToUpdate)
        {
            _efContext.Set<TEntity>().Update(entityToUpdate);
            await _efContext.SaveChangesAsync();
        }

        public void Delete(TEntity entityToDelete)
        {
            _efContext.Set<TEntity>().Remove(entityToDelete);
            _efContext.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entityToDelete)
        {
            _efContext.Set<TEntity>().Remove(entityToDelete);
            await _efContext.SaveChangesAsync();
        }
    }

}
