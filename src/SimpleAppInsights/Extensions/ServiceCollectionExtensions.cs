using System;
using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;

namespace SimpleAppInsights.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Based on: https://docs.microsoft.com/en-us/azure/application-insights/application-insights-console
        /// </summary>
        /// <param name="services"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="applicationName"></param>
        /// <param name="typeFromEntryAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurableApplicationInsights(
            this IServiceCollection services,
            ILogger logger,
            IConfiguration configuration,
            string applicationName,
            Type typeFromEntryAssembly)
        {
            var applicationInsightsOptions = configuration.GetApplicationInsightsOptions(typeFromEntryAssembly);

            services.ConfigureTelemetryChannelStorageFolder(
                logger,
                applicationInsightsOptions.TelemetryChannel.StorageFolder,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            TelemetryConfiguration.Active.InstrumentationKey = applicationInsightsOptions.InstrumentationKey;
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = applicationInsightsOptions.TelemetryChannel.DeveloperMode;

            var client = new TelemetryClient();
            services.AddSingleton(client);

            var applicationDescriptor = new ApplicationDescriptor(applicationName, applicationInsightsOptions.ApplicationVersion);
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ApplicationInitializer(applicationDescriptor));

            return services;
        }

        public static IServiceCollection ConfigureTelemetryChannelStorageFolder(
            this IServiceCollection services,
            ILogger logger,
            string storageFolder,
            bool isWindows)
        {
            if (isWindows ||
                string.IsNullOrWhiteSpace(storageFolder))
            {
                return services;
            }

            services.AddSingleton(typeof(ITelemetryChannel),
                new ServerTelemetryChannel {StorageFolder = storageFolder});

            logger.LogDebug("Configuring Application Insights storage folder with {TelemetryChannelStorageFolder}",
                storageFolder);

            return services;
        }
    }
}