using Polly;
using Refit;
using System.Net;

namespace TechChallenge.Contact.Integration.Service
{
    public class IntegrationService : IIntegrationService
    {
        public async Task<T?> SendResilientRequest<T>(Func<Task<T>> call)
        {
      
            var retryPolicy = Policy
                .HandleInner<ApiException>(ex => ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(
                    retryCount: 50000,
                    sleepDurationProvider: _ => TimeSpan.FromMilliseconds(2000)
                );

            var result = await retryPolicy.ExecuteAndCaptureAsync(call);

            if (result.Outcome == OutcomeType.Failure)
            {
                return default;
            }

            return result.Result;
        }
    }
}
