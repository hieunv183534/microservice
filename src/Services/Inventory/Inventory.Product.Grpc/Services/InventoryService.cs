using Grpc.Core;
using Inventory.Product.Grpc.Protos;
using Inventory.Product.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Inventory.Product.Grpc.Services;

public class InventoryService : StockProtoService.StockProtoServiceBase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger _logger;
    
    public InventoryService(IInventoryRepository inventoryRepository, ILogger logger)
    {
        _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
    {
        _logger.Information($"BEGIN GetStock ItemNo: {request.ItemNo}");
        var stockQuantity = await _inventoryRepository.GetStockQuantity(request.ItemNo);
        var result = new StockModel
        {
            Quantity = stockQuantity,
        };
        
        _logger.Information($"END GetStock ItemNo: {request.ItemNo} - Quantity: {stockQuantity}");

        return result;
    }

}