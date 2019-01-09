using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleWorker.Extensions;
using Serilog;

namespace SampleWorker
{
    class Program
    {
        /// <summary>
        /// Based on: https://github.com/Azure/azure-webjobs-sdk/blob/1bfb2c34be6f5d87fa9e65797f5d6d4fe2bda8b8/sample/SampleHost/Program.cs
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = IsDevelopment(environment);

            var builder = new HostBuilder()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration(b =>
                {
                    b.AddJsonFile("appsettings.json");

                    if (isDevelopment)
                    {
                        b.AddUserSecrets<Program>();
                    }

                    b.AddEnvironmentVariables();
                })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    var configuration = context.Configuration;

                    var serilogLevel = configuration.GetLoggingLevel();

                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.WithDemystifiedStackTraces()
                        .Enrich.FromLogContext();

                    var appInsightsIntrumentationKey = configuration.GetAppInsightsIntrumentationKey();

                    if (!string.IsNullOrEmpty(appInsightsIntrumentationKey))
                    {
                        loggingBuilder.Services.UseDeveloperApplicationInsights(appInsightsIntrumentationKey);

                        loggerConfiguration = loggerConfiguration
                            .WriteTo.ApplicationInsightsTraces(appInsightsIntrumentationKey, serilogLevel);
                    }
                    else
                    {
                        DisableApplicationInsights();
                    }

                    if (isDevelopment)
                    {
                        loggerConfiguration = loggerConfiguration
                            .WriteTo.Seq("http://localhost:5341/", serilogLevel)
                            .WriteTo.Console(serilogLevel);
                    }

                    var logger = loggerConfiguration.CreateLogger();

                    var loggerFactory = new LoggerFactory();
                    loggerFactory.AddSerilog(logger);

                    loggingBuilder.Services.AddLogging(loggerFactory);
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddOptions(context.Configuration)
                        .AddEventHandlers()
                        .AddTelemetry("AppServiceWebJob");

                    var webJobBuilder = services.AddWebJobs(o => { });
                    webJobBuilder.AddServiceBus(options =>
                    {
                        options.ConnectionString = context.Configuration["ServiceBus:ConnectionString"];
                    });

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, JobHostService>());
                    services.TryAddSingleton<MessagingProvider>();
                });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }

//        private static JobHostConfiguration GetJobHostConfiguration(IServiceProvider services)
//        {
//            var configuration = new JobHostConfiguration();
//
//            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
//            configuration.Queues.BatchSize = 1;
//            configuration.Queues.MaxDequeueCount = 10;
//            configuration.Queues.VisibilityTimeout = TimeSpan.FromSeconds(30);
//            configuration.JobActivator = new ContainerJobActivator(services);
//            configuration.LoggerFactory = services.GetService<ILoggerFactory>();
//
//            var webJobOptions = services.GetService<IOptions<AzureWebJobOptions>>().Value;
//
//            configuration.DashboardConnectionString = webJobOptions.DashboardConnectionString;
//            configuration.StorageConnectionString = webJobOptions.StorageConnectionString;
//
//            configuration.Tracing.ConsoleLevel = TraceLevel.Off;
//
//            var serviceBusConfiguration = new ServiceBusConfiguration
//            {
//                ConnectionString = webJobOptions.ServiceBusConnectionString
//            };
//
//            configuration.UseServiceBus(serviceBusConfiguration);
//
//            return configuration;
//        }

        private static void DisableApplicationInsights()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }

        private static bool IsDevelopment(string environment)
        {
            return "Development".Equals(environment);
        }
    }
}
