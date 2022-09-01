using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;

namespace Infrastructure.Policies;

public static class RetryPolicy
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
        builder.AddPolicyHandler(LinearHttpRetry());
    }

    private static IAsyncPolicy<HttpResponseMessage> ImmediateHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3,
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });

    private static AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });

    public static AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });
}