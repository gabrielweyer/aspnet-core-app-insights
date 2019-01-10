using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleWorker.Handlers;
using SampleWorker.Options;

namespace SampleWorker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services.AddSingleton(loggerFactory);
            services.AddLogging();

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddOptions();

            services.Configure<AzureWebJobOptions>(configuration.GetSection("AzureWebJob"));

            return services;
        }

        public static IServiceCollection AddEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<EventOneHandler>();
            services.AddScoped<EventTwoHandler>();
            services.AddScoped<EventThreeHandler>();

            return services;
        }
    }
}
