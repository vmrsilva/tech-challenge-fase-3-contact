using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechChallenge.Infrastructure.Context;

namespace TechChallenge.Tests.IntegrationTests.Setup
{
    public class BaseIntegrationTest : IClassFixture<TechChallangeApplicationFactory>, IDisposable
    {
        private readonly IServiceScope _scope;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;        
        protected readonly TechChallangeContext _dbContext;

        public BaseIntegrationTest(TechChallangeApplicationFactory factory)
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            _scope = factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<TechChallangeContext>();
           
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
