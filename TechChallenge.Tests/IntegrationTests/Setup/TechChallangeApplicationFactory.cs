using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Runtime.InteropServices;
using TechChallenge.Contact.Integration.Service;
using TechChallenge.Contact.Tests.Util;
using TechChallenge.Domain.Cache;
using TechChallenge.Domain.Contact.Entity;
using TechChallenge.Infrastructure.Cache;
using TechChallenge.Infrastructure.Context;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace TechChallenge.Tests.IntegrationTests.Setup
{
    public class TechChallangeApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;
        private readonly RedisContainer _redisContainer;
        private readonly Mock<IIntegrationService> _integrationServiceMock;

        public TechChallangeApplicationFactory()
        {
            _integrationServiceMock = new Mock<IIntegrationService>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _msSqlContainer = new MsSqlBuilder()
                    .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                      .WithPassword("password(!)Strong")
                             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                             .Build();
            }
            else
            {
                _msSqlContainer = new MsSqlBuilder().Build();
            }

            _redisContainer = new RedisBuilder().Build();            
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ConfigureDbContext(services);
                ConfigureCache(services);

                MockServices(services);
            });

            //builder.UseEnvironment("Development");
            base.ConfigureWebHost(builder);
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TechChallangeContext));
            if (context != null)
            {
                services.Remove(context);
                var options = services.Where(r => r.ServiceType == typeof(DbContextOptions)
                  || r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)).ToArray();
                foreach (var option in options)
                {
                    services.Remove(option);
                }
            }

            services.AddDbContext<TechChallangeContext>(options =>
            {

                var connectionString = _msSqlContainer.GetConnectionString();

                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(); // Habilita retry automático
                });

            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<TechChallangeContext>();
                dbContext.Database.Migrate();

                SeedRegion(dbContext);
            }
        }

        private void ConfigureCache(IServiceCollection services)
        {
            var cache = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDistributedCache));
            if (cache != null)
            {
                services.Remove(cache);
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
            });


            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ICacheWrapper, CacheWrapper>();
        }

        private void MockServices(IServiceCollection services)
        {
            var integration = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IIntegrationService));
            if (integration != null)
            {
                services.Remove(integration);
            }

            services.AddSingleton<IIntegrationService>(_integrationServiceMock.Object);
        }

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();

            await _redisContainer.StartAsync();
        }

        public async new Task DisposeAsync()
        {
            await _msSqlContainer.StopAsync();
            await _redisContainer.StopAsync();
        }

        public Mock<IIntegrationService> GetIntegrationServiceMock()
        {
            return _integrationServiceMock;
        }

        private void SeedRegion(TechChallangeContext context)
        {
            var contactOne = new ContactEntity("Test", "4141-3338", "test@email.com", Util.dddSp);
            var contactTwo = new ContactEntity("Test", "4747-4747", "test@email.com", Util.dddPr);
            context.Contact.AddRange(contactOne, contactTwo);

            context.SaveChanges();
        }
    }
}
