using Polly;
using Refit;
using System.Net.Sockets;

namespace TechChallenge.Contact.Integration.Service
{
    public class IntegrationService : IIntegrationService
    {
        public async Task<T?> SendResilientRequest<T>(Func<Task<T>> call)
        {

            var retryPolicy = Policy
                //.HandleInner<Exception>()
                .HandleInner<HttpRequestException>(ex =>
                ex.InnerException is SocketException socketEx &&
                socketEx.SocketErrorCode == SocketError.ConnectionRefused)
            .Or<ApiException>(e => e.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: _ => TimeSpan.FromMilliseconds(3000)
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
