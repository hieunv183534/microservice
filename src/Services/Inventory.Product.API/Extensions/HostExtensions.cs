using Inventory.Product.API.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Inventory.Product.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var settings = services.GetService<DatabaseSettings>();
        var mongoClient = services.GetRequiredService<IMongoClient>();
        new InventoryContextSeed()
            .SeedDataAsync(mongoClient, settings)
            .Wait();
        return host;
    }
}