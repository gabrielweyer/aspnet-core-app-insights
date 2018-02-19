using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleWorker.Extensions;
using Serilog;

namespace SampleWorker.Infrastructure
{
    public class ServiceProviderConfigurator
    {
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var isDevelopment = IsDevelopment();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            if (isDevelopment)
            {
                configurationBuilder = configurationBuilder
                    .AddUserSecrets<Program>();
            }

            var configuration = configurationBuilder
                .AddEnvironmentVariables()
                .Build();

            var serilogLevel = configuration.GetLoggingLevel();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithDemystifiedStackTraces()
                .Enrich.FromLogContext();

            var appInsightsIntrumentationKey = configuration.GetAppInsightsIntrumentationKey();

            if (!string.IsNullOrEmpty(appInsightsIntrumentationKey))
            {
                services.UseDeveloperApplicationInsights(appInsightsIntrumentationKey);

                loggerConfiguration = loggerConfiguration
                    .WriteTo.ApplicationInsightsTraces(appInsightsIntrumentationKey, serilogLevel);
            }
            else
            {
                DisableApplicationInsights();
            }

            if (isDevelopment)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Seq("http://localhost:5341/", serilogLevel)
                    .WriteTo.Console(serilogLevel);
            }

            var logger = loggerConfiguration.CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(logger);

            services
                .AddOptions(configuration)
                .AddLogging(loggerFactory)
                .AddEventHandlers();

            return services.BuildServiceProvider();
        }

        private static void DisableApplicationInsights()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }

        private static bool IsDevelopment()
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = "Development".Equals(environment);
            return isDevelopment;
        }
    }
}
