using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinephoriaServer.API.Repository.EntityFramwork
{
    public class EFRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EFRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // Helper to choose between tracking and no-tracking
        private IQueryable<TEntity> ApplyTracking(IQueryable<TEntity> query, bool trackChanges)
        {
            return trackChanges ? query : query.AsNoTracking();
        }

        // Basic queries
        public List<TEntity> GetAll(bool trackChanges = false)
        {
            return ApplyTracking(_dbSet, trackChanges).ToList();
        }

        public async Task<List<TEntity>> GetAllAsync(bool trackChanges = false)
        {
            return await ApplyTracking(_dbSet, trackChanges).ToListAsync();
        }

        public TEntity? GetById(int id, bool trackChanges = false)
        {
            var entity = _dbSet.Find(id);
            if (entity == null || !trackChanges) return entity;
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public async Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null || !trackChanges) return entity;
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        // Filtered queries
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
        {
            return ApplyTracking(_dbSet.Where(predicate), trackChanges).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
        {
            return await ApplyTracking(_dbSet.Where(predicate), trackChanges).ToListAsync();
        }

        // Basic commands
        public void Create(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        // Other utilities
        public int Count()
        {
            return _dbSet.Count();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
