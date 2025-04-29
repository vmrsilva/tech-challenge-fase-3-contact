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

        [Fact(DisplayName = "SendResilientRequest When Call Fails With Http Request Exception Returns Default")]
        public async Task SendResilientRequestWhenCallFailsWithHttpRequestExceptionReturnsDefault()
        {
            Func<Task<string>> call = () => throw new HttpRequestException("Simulated HTTP failure");

            var result = await _integrationService.SendResilientRequest(call);

            Assert.Null(result);
        }

        public async Task SendResilientRequestWhenCallFailsWithNonHttpExceptionThrowsException()
        {
            // Arrange
            Func<Task<string>> call = () => throw new InvalidOperationException("Simulated non-HTTP failure");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _integrationService.SendResilientRequest(call));
        }

        [Fact(DisplayName = "SendResilientRequest When Http Request Exception Occurs Retries Three Times")]
        public async Task SendResilientRequestWhenHttpRequestExceptionOccursRetriesThreeTimes()
        {
            int retryCount = 0;
            Func<Task<string>> call = () =>
            {
                retryCount++;
                throw new HttpRequestException("Simulated HTTP failure for retry test");
            };

            var result = await _integrationService.SendResilientRequest(call);

            Assert.Null(result);
            Assert.Equal(4, retryCount);
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
                    throw new HttpRequestException("Temporary HTTP failure");
                }
                return Task.FromResult("Recovered Success");
            };

            var result = await _integrationService.SendResilientRequest(call);

            Assert.NotNull(result);
            Assert.Equal("Recovered Success", result);
            Assert.Equal(3, retryCount);
        }
    }
}
