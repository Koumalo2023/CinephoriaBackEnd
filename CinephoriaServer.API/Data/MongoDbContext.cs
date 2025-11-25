using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.MongooDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CinephoriaServer.API.Data
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

        // Méthode générique pour obtenir une collection
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        //Collections pour des types spécifiques si nécessaire
        public IMongoCollection<AdminDashboard> AdminDashboards => _database.GetCollection<AdminDashboard>("admin_dashboard");
       
    }
}
