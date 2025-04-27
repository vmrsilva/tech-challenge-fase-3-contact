using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Contact.Integration.Service;
using TechChallenge.Domain.Base.Repository;
using TechChallenge.Domain.Cache;
using TechChallenge.Domain.Contact.Repository;
using TechChallenge.Domain.Contact.Service;
using TechChallenge.Infrastructure.Cache;
using TechChallenge.Infrastructure.Context;
using TechChallenge.Infrastructure.Repository.Base;
using TechChallenge.Infrastructure.Repository.Contact;

namespace TechChallenge.IoC
{
    public static class DomainInjection
    {
        public static void AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureContext(services, configuration);
            ConfigureBase(services);
            //ConfigureRegion(services);
            ConfigureContact(services);
            ConfigureCache(services, configuration);
            ConfigureIntegration(services);
        }

        public static void ConfigureContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TechChallangeContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<TechChallangeContext>();
             //   dbContext.Database.Migrate();
            }
        }

        public static void ConfigureBase(IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }

        public static void ConfigureContact(IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactService, ContactService>();
        }
        public static void ConfigureCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options => {
                options.InstanceName = nameof(CacheRepository);
                options.Configuration = configuration.GetConnectionString("Cache");
            });
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ICacheWrapper, CacheWrapper>();
        }

        private static void ConfigureIntegration(IServiceCollection services)
        {
            services.AddScoped<IIntegrationService, IntegrationService>();
        }
    }
}
