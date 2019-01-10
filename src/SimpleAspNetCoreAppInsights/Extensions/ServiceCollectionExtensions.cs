using System;
using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleAppInsights.Extensions;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;

namespace SimpleAspNetCoreAppInsights.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurableAspNetCoreApplicationInsights(
            this IServiceCollection services,
            ILogger logger,
            IConfiguration configuration,
            string applicationName,
            Type typeFromEntryAssembly)
        {
            var applicationInsightsOptions = configuration.GetApplicationInsightsAspNetCoreOptions(typeFromEntryAssembly);

            services.ConfigureTelemetryChannelStorageFolder(
                logger,
                applicationInsightsOptions.TelemetryChannel.StorageFolder,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ApplicationVersion = applicationInsightsOptions.ApplicationVersion;
                options.EnableAdaptiveSampling = applicationInsightsOptions.EnableAdaptiveSampling;
                options.InstrumentationKey = applicationInsightsOptions.InstrumentationKey;
                options.DeveloperMode = applicationInsightsOptions.TelemetryChannel.DeveloperMode;
            });

            var applicationDescriptor = new ApplicationDescriptor(applicationName, applicationInsightsOptions.ApplicationVersion);
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ApplicationInitializer(applicationDescriptor));

            return services;
        }
    }
}