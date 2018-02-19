using System;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace SampleWorker.Extensions
{
    public static class ConfigurationRootExtensions
    {
        public static string GetAppInsightsIntrumentationKey(this IConfigurationRoot configuration)
        {
            return configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
        }

        public static LogEventLevel GetLoggingLevel(this IConfigurationRoot configuration)
        {
            return configuration.GetLoggingLevel("MinimumLevel:Default");
        }

        private static LogEventLevel GetLoggingLevel(
            this IConfigurationRoot configuration,
            string keyName,
            LogEventLevel defaultLevel = LogEventLevel.Warning)
        {
            try
            {
                return configuration.GetValue($"Serilog:{keyName}", defaultLevel);
            }
            catch (Exception)
            {
                return defaultLevel;
            }
        }
    }
}
