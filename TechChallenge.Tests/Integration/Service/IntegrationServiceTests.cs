using Refit;
using System.Net.Sockets;
using System.Net;
using TechChallenge.Contact.Integration.Service;

namespace TechChallenge.Contact.Tests.Integration.Service
{
    public class IntegrationServiceTests
    {
        private readonly IntegrationService _integrationService;

        public IntegrationServiceTests()
        {
            _integrationService = new IntegrationService();
        }

        [Fact(DisplayName = "SendResilientRequest When Call Succeeds Returns Result")]
        public async Task SendResilientRequestWhenCallSucceedsReturnsResult()
        {
            var expectedResult = "Success";
            Func<Task<string>> call = () => Task.FromResult(expectedResult);

            var result = await _integrationService.SendResilientRequest(call);

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "SendResilientRequest When Call Fails With Retryable HttpRequestException Returns Default")]
        public async Task SendResilientRequestWhenCallFailsWithRetryableHttpRequestExceptionReturnsDefault()
        {
            Func<Task<string>> call = () =>
                throw new HttpRequestException("Simulated HTTP failure",
                    new SocketException((int)SocketError.ConnectionRefused));

            var result = await _integrationService.SendResilientRequest(call);

            Assert.Null(result);
        }

        [Fact(DisplayName = "SendResilientRequest When Call Fails With ApiException 503 Returns Default")]
        public async Task SendResilientRequestWhenCallFailsWithApiException503ReturnsDefault()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://fake-url.com")
            };

            var apiException = await ApiException.Create(
                responseMessage.RequestMessage,
                HttpMethod.Get,
                responseMessage,
                new RefitSettings()
            );

            Func<Task<string>> call = () => throw apiException;

            var result = await _integrationService.SendResilientRequest(call);

            Assert.Null(result);
        }


        [Fact(DisplayName = "SendResilientRequest When Retryable Exception Occurs Retries Three Times")]
        public async Task SendResilientRequestWhenRetryableExceptionOccursRetriesThreeTimes()
        {
            int retryCount = 0;
            Func<Task<string>> call = () =>
            {
                retryCount++;
                throw new HttpRequestException("Simulated retryable HTTP failure",
                    new SocketException((int)SocketError.ConnectionRefused));
            };

            var result = await _integrationService.SendResilientRequest(call);

            Assert.Null(result);
            Assert.Equal(4, retryCount); // 1 tentativa + 3 retries
        }

        [Fact(DisplayName = "SendResilientRequest Succeeds After Retries")]
        public async Task SendResilientRequestSucceedsAfterRetries()
        {
            int retryCount = 0;
            Func<Task<string>> call = () =>
            {
                retryCount++;
                if (retryCount < 3)
                {
                    throw new HttpRequestException("Temporary HTTP failure",
                        new SocketException((int)SocketError.ConnectionRefused));
                }
                return Task.FromResult("Recovered Success");
            };

            var result = await _integrationService.SendResilientRequest(call);

            Assert.NotNull(result);
            Assert.Equal("Recovered Success", result);
            Assert.Equal(3, retryCount); // sucesso na terceira tentativa
        }
    }
}
