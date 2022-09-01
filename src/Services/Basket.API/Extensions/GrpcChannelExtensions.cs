using Grpc.Core;
using Grpc.Net.Client.Configuration;

namespace Basket.API.Extensions;

public static class GrpcChannelExtensions
{
    public static void ConfigureGrpcChannelOptions(this IHttpClientBuilder builder)
    {
        builder.ConfigureChannel(options =>
        {
            options.ServiceConfig = new ServiceConfig
            {
                MethodConfigs = { getDefaultMethodConfig() }
            };
        });
    }
    
    private static MethodConfig getDefaultMethodConfig()
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes =
                {
                    // Whatever status codes we want to look for
                    StatusCode.Unauthenticated, StatusCode.NotFound, StatusCode.Unavailable,
                }
            }
        };

        return defaultMethodConfig;
    }
}