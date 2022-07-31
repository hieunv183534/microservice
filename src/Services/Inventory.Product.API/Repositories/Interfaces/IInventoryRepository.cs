using Inventory.Product.API.Entities;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IInventoryRepository
{
    Task<IEnumerable<InventoryEntry>> GetProductInventories(string itemNo);
}