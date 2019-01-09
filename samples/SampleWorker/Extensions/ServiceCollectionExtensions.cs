using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleWorker.Handlers;
using SampleWorker.Options;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;

namespace SampleWorker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Based on: https://docs.microsoft.com/en-us/azure/application-insights/application-insights-console
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instrumentationKey"></param>
        /// <returns></returns>
        public static IServiceCollection UseDeveloperApplicationInsights(
            this IServiceCollection services,
            string instrumentationKey)
        {
            TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            var client = new TelemetryClient();
            services.AddSingleton(client);

            return services;
        }

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

        public static IServiceCollection AddTelemetry(this IServiceCollection services, string applicationName)
        {
            services.AddSingleton(new ApplicationDescriptor(applicationName, ApplicationDescriptor.GetAssemblyInformationalVersion(typeof(Program))));
            services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

            return services;
        }
    }
}
