using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Basket.API.Extensions;

public static class RetryPolicyExtensions
{
    public static IHttpClientBuilder ConfigPolicyHandler(this IHttpClientBuilder builder)
        => builder.AddPolicyHandler(GetLinearHttpRetryPolicy());

    private static IAsyncPolicy<HttpResponseMessage> GetExponentialHttpRetryPolicy()
    {
        //  2 ^ 1 = 2 seconds then
        //  2 ^ 2 = 4 seconds then
        //  2 ^ 3 = 8 seconds then
        //  2 ^ 4 = 16 seconds then
        //  2 ^ 5 = 32 seconds

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetImmediateHttpRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(
                3,
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetLinearHttpRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(3),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });
    }
}