using Inventory.Product.API.Entities;
using Inventory.Product.API.Repositories.Abstractions;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IInventoryMongoDbRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntry>> GetAllByItemNoAsync(string itemNo);
    Task<InventoryEntry> GetAllByIdAsync(string id);
}