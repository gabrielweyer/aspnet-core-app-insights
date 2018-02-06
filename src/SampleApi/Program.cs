using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace SampleApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseAzureAppServices();

            var contentRoot = webHostBuilder.GetSetting(WebHostDefaults.ContentRootKey);
            var environment = webHostBuilder.GetSetting(WebHostDefaults.EnvironmentKey);

            var isDevelopment = EnvironmentName.Development.Equals(environment);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();

            var serilogLevel = configuration.GetLoggingLevel("MinimumLevel:Default");

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithDemystifiedStackTraces();

            var appInsightsIntrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

            if (!string.IsNullOrEmpty(appInsightsIntrumentationKey))
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.ApplicationInsightsTraces(appInsightsIntrumentationKey, serilogLevel);

                TelemetryConfiguration.Active.InstrumentationKey = appInsightsIntrumentationKey;
            }
            else
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }

            if (isDevelopment)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Seq("http://localhost:5341/")
                    .WriteTo.Console(serilogLevel);
            }

            var logger = loggerConfiguration.CreateLogger();

            try
            {
                logger.Information("Starting Host...");

                return webHostBuilder
                    .UseStartup<Startup>()
                    .UseConfiguration(configuration)
                    .UseSerilog(logger, true)
                    .Build();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
        }
    }

    internal static class ConfigurationRootExtensions
    {
        internal static LogEventLevel GetLoggingLevel(this IConfigurationRoot configuration, string keyName,
            LogEventLevel defaultLevel = LogEventLevel.Warning)
        {
            try
            {
                return configuration.GetValue($"Serilog:{keyName}", LogEventLevel.Warning);
            }
            catch (Exception)
            {
                return defaultLevel;
            }
        }
    }
}
