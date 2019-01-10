using System;
using Microsoft.Extensions.Configuration;
using SimpleAppInsights.Options;
using SimpleInstrumentation.Models;

namespace SimpleAppInsights.Extensions
{
    public static class ConfigurationExtensions
    {
        public static ApplicationInsightsSimpleOptions GetApplicationInsightsOptions(
            this IConfiguration configuration,
            Type type)
        {
            var options = configuration
                              .GetSection("ApplicationInsights")
                              .Get<ApplicationInsightsSimpleOptions>() ?? new ApplicationInsightsSimpleOptions();

            if (string.IsNullOrWhiteSpace(options.ApplicationVersion))
            {
                options.ApplicationVersion = ApplicationDescriptor.GetAssemblyInformationalVersion(type);
            }

            return options;
        }

        public static string GetApplicationInsightsInstrumentationKey(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
        }
    }
}