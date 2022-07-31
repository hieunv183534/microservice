using Inventory.Product.API.Entities.Abstraction;
using Inventory.Product.API.Extensions;
using MongoDB.Driver;

namespace Inventory.Product.API.Repositories;

public class MongoDbRepository
{
    public IMongoDatabase Database { get; }

    public MongoDbRepository(IMongoClient client, DatabaseSettings settings)
    {
        Database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(ReadPreference? readPreference = null)
        where T : MongoEntity
    {
        return Database
            .WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName<T>());
    }

    private static string GetCollectionName<T>()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() as
            BsonCollectionAttribute)?.CollectionName;
    }
}