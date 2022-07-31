using Inventory.Product.API.Entities;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IInventoryMongoDbRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntry>> GetProductInventories(string itemNo);
    Task<InventoryEntry> GetProductInventoriesByIdAsync(string id);
}