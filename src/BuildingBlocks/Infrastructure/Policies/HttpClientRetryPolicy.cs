using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static void UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(ImmediateHttpRetry());
    }
    
    public static void UseLinearHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(LinearHttpRetry());
    }
    
    public static void UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(ExponentialHttpRetry());
    }

    private static IAsyncPolicy<HttpResponseMessage> ImmediateHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3, (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                          $"{context.OperationKey}, due to: {exception.Exception.Message}");
            });
    
    private static IAsyncPolicy<HttpResponseMessage> LinearHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3),
                (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                          $"{context.OperationKey}, due to: {exception.Exception.Message}");
            });
    
    
    private static IAsyncPolicy<HttpResponseMessage> ExponentialHttpRetry() =>
        // In this case will wait for
        //  2 ^ 1 = 2 seconds then
        //  2 ^ 2 = 4 seconds then
        //  2 ^ 3 = 8 seconds then
        //  2 ^ 4 = 16 seconds then
        //  2 ^ 5 = 32 seconds
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt 
                    => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                          $"{context.OperationKey}, due to: {exception.Exception.Message}");
            });
}