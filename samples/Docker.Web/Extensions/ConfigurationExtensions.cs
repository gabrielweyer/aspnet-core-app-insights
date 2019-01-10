using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SimpleAppInsights.Extensions;

namespace Docker.Web.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static Logger GetSerilogLogger(this IConfiguration configuration, bool isDevelopment)
        {
            var serilogDefaultMinimumLevel = configuration.GetSerilogLoggingLevel("MinimumLevel:Default");

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext();

            if (isDevelopment)
            {
                loggerConfiguration.WriteTo.Console(serilogDefaultMinimumLevel);
            }

            var applicationInsightsInstrumentationKey = configuration.GetApplicationInsightsInstrumentationKey();

            if (!string.IsNullOrWhiteSpace(applicationInsightsInstrumentationKey))
            {
                loggerConfiguration.WriteTo.ApplicationInsightsTraces(applicationInsightsInstrumentationKey);
            }

            var logger = loggerConfiguration.CreateLogger();
            Log.Logger = logger;

            return logger;
        }

        private static LogEventLevel GetSerilogLoggingLevel(
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