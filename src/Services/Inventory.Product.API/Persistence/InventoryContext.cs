using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Persistence.Interfaces;
using MongoDB.Driver;

namespace Inventory.Product.API.Persistence;

public class InventoryContext : IInventoryContext
{
    private readonly DatabaseSettings _settings;
    private readonly IMongoClient _mongoClient;
    
    public InventoryContext(IMongoClient mongoClient, DatabaseSettings settings)
    {
        _settings = settings;
        _mongoClient = mongoClient;

        if (!_mongoClient.GetDatabase(_settings.DatabaseName)
                .ListCollectionNames().ToList()
                .Contains("InventoryEntry"))
        {
            _mongoClient.GetDatabase(_settings.DatabaseName).CreateCollection("InventoryEntry");
        }

        InventoryEntries = mongoClient.GetDatabase(_settings.DatabaseName).GetCollection<InventoryEntry>("InventoryEntry");
    }
    
    public IMongoCollection<InventoryEntry> InventoryEntries { get; }
}