using System;
using Microsoft.Extensions.Configuration;
using SimpleAppInsights.Extensions;
using SimpleAspNetCoreAppInsights.Options;

namespace SimpleAspNetCoreAppInsights.Extensions
{
    public static class ConfigurationExtensions
    {
        public static ApplicationInsightsAspNetCoreSimpleOptions GetApplicationInsightsAspNetCoreOptions(
            this IConfiguration configuration,
            Type type)
        {
            ApplicationInsightsAspNetCoreSimpleOptions options = configuration.GetApplicationInsightsOptions(type);

            var enableAdaptiveSamplingFromConfig = configuration["ApplicationInsights:EnableAdaptiveSampling"];

            if (bool.TryParse(enableAdaptiveSamplingFromConfig, out var enableAdaptiveSampling))
            {
                options.EnableAdaptiveSampling = enableAdaptiveSampling;
            }

            return options;
        }
    }
}