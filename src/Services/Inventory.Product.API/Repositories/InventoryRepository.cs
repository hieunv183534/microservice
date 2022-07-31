using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace Inventory.Product.API.Repositories;

public class InventoryRepository :  IInventoryRepository
{
    // public InventoryRepository(IMongoClient client, DatabaseSettings settings) : base(client, settings)
    // {
    // }

    // public async Task<IEnumerable<InventoryEntry>> GetInventories(string itemNo)
    // {
    //     var result = await GetCollection<InventoryEntry>()
    //         .Find(x => x.ItemNo.Equals(itemNo))
    //         .ToListAsync();
    //
    //     return result;
    // }
    public Task<IEnumerable<InventoryEntry>> GetInventories(string itemNo)
    {
        throw new NotImplementedException();
    }
}