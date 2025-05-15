using Polly;
using Refit;
using System.Net;
using System.Net.Sockets;

namespace TechChallenge.Contact.Integration.Service
{
    public class IntegrationService : IIntegrationService
    {
        public async Task<T?> SendResilientRequest<T>(Func<Task<T>> call)
        {

            var retryPolicy = Policy
                .HandleInner<HttpRequestException>(ex =>
                ex.InnerException is SocketException socketEx &&
                  (
                socketEx.SocketErrorCode == SocketError.ConnectionRefused ||
                socketEx.SocketErrorCode == SocketError.HostNotFound        
            )
        )

                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: _ => TimeSpan.FromMilliseconds(4000)
                );
            
            var result = await retryPolicy.ExecuteAndCaptureAsync(call);

            if (result.Outcome == OutcomeType.Failure)
            {

                if (result.FinalException is ApiException apiEx)
                {
                    var statusCode = apiEx.StatusCode;
                    
                    if (statusCode == HttpStatusCode.BadRequest)
                    {
                        return default;
                    }
                }

                    throw new HttpRequestException("Um serviço externo está indisponível no momento.");
            }

            return result.Result;
        }
    }
}
