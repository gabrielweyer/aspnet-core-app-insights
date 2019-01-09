using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;

namespace Docker.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurableApplicationInsightsTelemetry(
            this IServiceCollection services,
            ILogger logger,
            IConfiguration configuration,
            string applicationName)
        {
            services.ConfigureTelemetryChannelStorageFolder(logger, configuration);

            var applicationVersion = configuration["ApplicationInsights:ApplicationVersion"];

            if (string.IsNullOrWhiteSpace(applicationVersion))
            {
                applicationVersion = ApplicationDescriptor.GetAssemblyInformationalVersion(typeof(Startup));
            }

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ApplicationVersion = applicationVersion;

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

            var applicationDescriptor = new ApplicationDescriptor(applicationName, applicationVersion);
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ApplicationInitializer(applicationDescriptor));

            return services;
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