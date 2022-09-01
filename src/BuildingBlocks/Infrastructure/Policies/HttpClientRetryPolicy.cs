using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureImmediateHttpRetry());
    }

    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureLinearHttpRetry());
    }
    
    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureExponentialHttpRetry());
    }

    public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy());
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3, (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                          $"{context.OperationKey}, due to: {exception.Exception.Message}");
            });
    
    private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3),
                (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                          $"{context.OperationKey}, due to: {exception.Exception.Message}");
            });
    
    
    private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry() =>
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