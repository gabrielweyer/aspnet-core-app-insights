using System;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace SampleWorker.Extensions
{
    public static class ConfigurationRootExtensions
    {
        public static LogEventLevel GetLoggingLevel(this IConfiguration configuration)
        {
            return configuration.GetLoggingLevel("MinimumLevel:Default");
        }

        private static LogEventLevel GetLoggingLevel(
            this IConfiguration configuration,
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
