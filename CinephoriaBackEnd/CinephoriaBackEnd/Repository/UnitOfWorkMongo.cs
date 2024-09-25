using CinephoriaBackEnd.Models;
using MongoDB.Driver;

namespace CinephoriaBackEnd.Repository
{
    public class UnitOfWorkMongoDb : IUnitOfWorkMongoDb
    {
        private readonly MongoDbContext _context;

        public UnitOfWorkMongoDb(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Incident>> GetAllAsync()
        {
            return await _context.Incidents.Find(_ => true).ToListAsync();
        }

        public async Task<Incident?> GetByIdAsync(string id)
        {
            return await _context.Incidents.Find(incident => incident.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Incident incident)
        {
            await _context.Incidents.InsertOneAsync(incident);
        }

        public async Task UpdateAsync(Incident incident)
        {
            await _context.Incidents.ReplaceOneAsync(existingIncident => existingIncident.Id == incident.Id, incident);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.Incidents.DeleteOneAsync(incident => incident.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await Task.CompletedTask; // Peut être ignoré ou implémenté si vous avez une logique spécifique
        }

        public void Dispose(){}
    }

}
