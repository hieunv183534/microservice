using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace Inventory.Product.API.Repositories;

public class InventoryMongoDbRepository : MongoDbRepository<InventoryEntry>, IInventoryMongoDbRepository
{
    public InventoryMongoDbRepository(IMongoClient client, DatabaseSettings settings) : base(client, settings)
    {
    }

    public async Task<IEnumerable<InventoryEntry>> GetProductInventories(string itemNo)
    {
        var result = await FindAll()
            .Find(x => x.ItemNo.Equals(itemNo))
            .ToListAsync();

        return result;
    }

    public Task<InventoryEntry> GetProductInventoriesByIdAsync(string id)
    {
        FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(s => s.Id, id);
        return FindAll().Find(filter).FirstOrDefaultAsync();
    }
}