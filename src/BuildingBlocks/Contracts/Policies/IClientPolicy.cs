using Polly.Retry;

namespace Contracts.Policies;

public interface IClientPolicy
{
    AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
    AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
    AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }
}