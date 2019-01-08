using System;
using Docker.Downstream.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace Docker.Downstream
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception e)
            {
                if (!(Log.Logger is Logger))
                {
                    AssignConsoleLoggerToStaticLogger();
                }

                Log.Logger.Fatal(e, "Host terminated unexpectedly");

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(args);

            var contentRoot = webHostBuilder.GetSetting(WebHostDefaults.ContentRootKey);
            var environment = webHostBuilder.GetSetting(WebHostDefaults.EnvironmentKey);
            var isDevelopment = EnvironmentName.Development.Equals(environment);

            var configuration = GetDefaultConfiguration(args, contentRoot, environment, isDevelopment);

            var logger = configuration.GetSerilogLogger(isDevelopment);

            return webHostBuilder
                .UseStartup<Startup>()
                .UseConfiguration(configuration)
                .UseSerilog(logger);
        }

        private static IConfigurationRoot GetDefaultConfiguration(
            string[] args,
            string contentRoot,
            string environment,
            bool isDevelopment)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{environment}.json", true, false);

            if (isDevelopment)
            {
                configurationBuilder.AddUserSecrets<Startup>(true);
            }

            configurationBuilder.AddEnvironmentVariables();

            if (args != null)
            {
                configurationBuilder.AddCommandLine(args);
            }

            return configurationBuilder.Build();
        }

        private static void AssignConsoleLoggerToStaticLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
