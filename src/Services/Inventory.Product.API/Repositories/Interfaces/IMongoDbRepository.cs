using Contracts.Domains;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IMongoDbRepository<T, in K> where T : EntityBase<K>
{
    IQueryable<T> FindAll(bool trackChanges = false);
}