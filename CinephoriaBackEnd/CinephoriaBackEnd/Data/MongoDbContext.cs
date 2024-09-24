using CinephoriaBackEnd.Configurations;
using CinephoriaBackEnd.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CinephoriaBackEnd
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Inject MongoDbSettings instead of MongoDbContext
    public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Collection for 'Incidents'
        public IMongoCollection<Incident> Incidents => _database.GetCollection<Incident>("Incidents");
    }
}
