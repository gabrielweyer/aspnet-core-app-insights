using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SampleApi.Extensions;
using Serilog;
using Serilog.Events;
using SimpleAppInsights.Extensions;

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

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json", false, false);

            if (isDevelopment)
            {
                configurationBuilder = configurationBuilder
                    .AddUserSecrets<Startup>();
            }

            var configuration = configurationBuilder
                .AddEnvironmentVariables()
                .Build();

            var serilogLevel = configuration.GetLoggingLevel("MinimumLevel:Default");

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithDemystifiedStackTraces();

            var appInsightsInstrumentationKey = configuration.GetApplicationInsightsInstrumentationKey();

            if (!string.IsNullOrEmpty(appInsightsInstrumentationKey))
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.ApplicationInsightsTraces(appInsightsInstrumentationKey, serilogLevel);

                webHostBuilder = webHostBuilder
                    .UseDeveloperApplicationInsights(appInsightsInstrumentationKey);
            }
            else
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }

            if (isDevelopment)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Seq("http://localhost:5341/", serilogLevel)
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
