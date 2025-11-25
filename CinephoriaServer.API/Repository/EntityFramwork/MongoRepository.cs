using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.MongooDb;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CinephoriaServer.API.Repository.EntityFramwork
{
    public class MongoRepository<T> : IMongoRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        // Méthode pour obtenir tous les éléments
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        // Méthode pour obtenir un élément par son Id
        public async Task<T> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();
        }

        // Méthode pour ajouter un élément
        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        // Méthode pour mettre à jour un élément
        public async Task UpdateAsync(T entity)
        {
            var objectId = typeof(T).GetProperty("Id")?.GetValue(entity) as ObjectId?;

            if (objectId.HasValue)
            {
                await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", objectId.Value), entity);
            }
            else
            {
                throw new Exception("L'objet ne possède pas d'Id valide.");
            }
        }

        // Méthode pour supprimer un élément par son Id
        public async Task<bool> DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new ArgumentException("L'ID spécifié est invalide.");
            }

            var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", objectId));
            return result.DeletedCount > 0;
        }

        // Méthode pour filtrer les éléments
        public async Task<List<T>> FilterAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<T>> FilterMovieAsync(
            FilterDefinition<T> filter,
            int page = 1,
            int pageSize = 10,
            SortDefinition<T> sort = null)
        {
            try
            {
                // Calculer le décalage pour la pagination
                var skip = (page - 1) * pageSize;

                // Appliquer le filtre, le tri et la pagination
                var query = _collection.Find(filter)
                                       .Skip(skip)
                                       .Limit(pageSize);

                // Appliquer le tri s'il est fourni
                if (sort != null)
                {
                    query = query.Sort(sort);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Gérer l'exception (par exemple, journaliser l'erreur)
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
                throw; // Vous pouvez relancer ou gérer l'exception selon le besoin
            }
        }


        // Méthode pour trouver des éléments en utilisant une expression lambda
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        // **Nouvelle Méthode**: Met à jour un élément spécifique en fonction d'un filtre et des champs à mettre à jour.
        public async Task UpdatePartialAsync(string id, UpdateDefinition<T> updates)
        {
            var objectId = ObjectId.Parse(id);
            await _collection.UpdateOneAsync(Builders<T>.Filter.Eq("_id", objectId), updates);
        }

        // **Nouvelle Méthode**: Récupérer un élément via un filtre.
        public async Task<T> FindOneAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // **Nouvelle Méthode**: Vérifier l'existence d'un élément avec un certain filtre.
        public async Task<bool> ExistsAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).AnyAsync();
        }
    }

}
