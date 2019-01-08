using System.Runtime.InteropServices;
using Docker.Downstream.Telemetry;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Docker.Downstream.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConfigurableApplicationInsightsTelemetry(
            this IServiceCollection services,
            ILogger logger,
            IConfiguration configuration)
        {
            services.ConfigureTelemetryChannelStorageFolder(logger, configuration);

            services.AddApplicationInsightsTelemetry(options =>
            {
                var applicationVersion = configuration["ApplicationInsights:ApplicationVersion"];

                if (!string.IsNullOrWhiteSpace(applicationVersion))
                {
                    options.ApplicationVersion = applicationVersion;
                }

                var enableAdaptiveSamplingFromConfig = configuration["ApplicationInsights:EnableAdaptiveSampling"];

                if (bool.TryParse(enableAdaptiveSamplingFromConfig, out var enableAdaptiveSampling))
                {
                    options.EnableAdaptiveSampling = enableAdaptiveSampling;
                }

                var instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];

                if (!string.IsNullOrWhiteSpace(instrumentationKey))
                {
                    options.InstrumentationKey = instrumentationKey;
                }

                var developerModeFromConfig = configuration["ApplicationInsights:TelemetryChannel:DeveloperMode"];

                if (bool.TryParse(developerModeFromConfig, out var developerMode))
                {
                    options.DeveloperMode = developerMode;
                }
            });

            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ServiceNameInitializer());
        }

        private static void ConfigureTelemetryChannelStorageFolder(this IServiceCollection services, ILogger logger,
            IConfiguration configuration)
        {
            var telemetryChannelStorageFolder = configuration["ApplicationInsights:TelemetryChannel:StorageFolder"];

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                string.IsNullOrWhiteSpace(telemetryChannelStorageFolder))
            {
                return;
            }

            services.AddSingleton(typeof(ITelemetryChannel),
                new ServerTelemetryChannel {StorageFolder = telemetryChannelStorageFolder});

            logger.LogDebug("Configuring Application Insights storage folder with {TelemetryChannelStorageFolder}",
                telemetryChannelStorageFolder);
        }
    }
}