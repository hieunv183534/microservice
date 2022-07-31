using System.Linq.Expressions;
using Contracts.Domains;
using Contracts.Domains.Interfaces;
using Inventory.Product.API.Entities.Abstraction;
using Inventory.Product.API.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Inventory.Product.API.Repositories;

public class MongoDbRepository<T, K> : IRepositoryQueryBase<T, K>  where T : EntityBase<K>
{
    public IMongoDatabase Database { get; }
    
    public MongoDbRepository(IMongoClient client, DatabaseSettings settings)
    {
        Database = client.GetDatabase(settings.DatabaseName);
    }
    //
    // public IMongoCollection<T> GetCollection<T>(ReadPreference? readPreference = null)
    //     where T : MongoEntity
    // {
    //     return Database
    //         .WithReadPreference(readPreference ?? ReadPreference.Primary)
    //         .GetCollection<T>(GetCollectionName<T>());
    // }
    //
    
    public static string GetCollectionName<T>()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() as
            BsonCollectionAttribute)?.CollectionName;
    }
    
    public IQueryable<T> FindAll(bool trackChanges = false)
    {
        return Database.WithReadPreference(ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName<T>())
            .AsQueryable();
    }

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync(K id)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }
}