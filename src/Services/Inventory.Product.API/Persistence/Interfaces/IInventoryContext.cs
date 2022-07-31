using Inventory.Product.API.Entities;
using MongoDB.Driver;

namespace Inventory.Product.API.Persistence.Interfaces;

public interface IInventoryContext
{
    IMongoCollection<InventoryEntry> InventoryEntries { get; }
}