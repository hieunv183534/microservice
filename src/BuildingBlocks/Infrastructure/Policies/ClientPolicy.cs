using Contracts.Policies;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Infrastructure.Policies;

public class ClientPolicy : IClientPolicy
{
    public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get;}
    public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry {get;}
    public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry {get;}

    public ClientPolicy()
    {
        ImmediateHttpRetry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(10);

        LinearHttpRetry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

        ExponentialHttpRetry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));            
    }
}