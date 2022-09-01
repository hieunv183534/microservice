using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Shared.Configurations;
using ILogger = Serilog.ILogger;

namespace Basket.API.GrpcServices;

using Inventory.Grpc.Client;

public class StockItemGrpcService
{
    private readonly ILogger _logger;
    private readonly GrpcSettings _grpcSettings;
    
    public StockItemGrpcService(ILogger logger, GrpcSettings grpcSettings)
    {
        _logger = logger;
        _grpcSettings = grpcSettings;
    }
    
    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            _logger.Information($"BEGIN: Get Stock StockItemGrpcService Item No: {itemNo}");
            var stockItemRequest = new GetStockRequest { ItemNo = itemNo };

            var channel = GrpcChannel.ForAddress(_grpcSettings.StockUrl, new GrpcChannelOptions
            {
                ServiceConfig = new ServiceConfig { MethodConfigs = { getDefaultMethodConfig() } }
            });
            // var channel = GrpcChannel.ForAddress(_grpcSettings.StockUrl);
            var client = new StockProtoService.StockProtoServiceClient(channel);

            var result = await client.GetStockAsync(stockItemRequest);
            _logger.Information($"END: Get Stock StockItemGrpcService Item No: {itemNo} - Stock value: {result.Quantity}");

            return result;
        }
        catch (RpcException e)
        {
            _logger.Error($"Grpc StockItemGrpcService failed : {e.Message}");
            return new StockModel
            {
                Quantity = -1
            };
        }
    }

    private MethodConfig getDefaultMethodConfig()
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes =
                {
                    // Whatever status codes we want to look for
                    StatusCode.Unauthenticated, 
                    StatusCode.NotFound, 
                    StatusCode.Unavailable,
                    StatusCode.DeadlineExceeded,
                    StatusCode.Internal,
                    StatusCode.NotFound,
                    StatusCode.ResourceExhausted,
                    StatusCode.Unavailable,
                    StatusCode.Unknown
                }
            }
        };

        return defaultMethodConfig;
    }
}