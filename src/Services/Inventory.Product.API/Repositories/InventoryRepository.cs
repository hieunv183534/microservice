using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace Inventory.Product.API.Repositories;

public class InventoryRepository : MongoDbRepository, IInventoryRepository
{
    public InventoryRepository(IMongoClient client, DatabaseSettings settings) : base(client, settings)
    {
    }

    public async Task<IEnumerable<InventoryEntry>> GetProductInventories(string itemNo)
    {
        var result = await GetCollection<InventoryEntry>()
            .Find(x => x.ItemNo.Equals(itemNo))
            .ToListAsync();

        return result;
    }
}