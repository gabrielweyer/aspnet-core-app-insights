using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SampleWorker.Infrastructure;
using SampleWorker.Options;

namespace SampleWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var services = ServiceProviderConfigurator.ConfigureServices())
            {
                var logger = services.GetService<ILogger<Program>>();
                logger.LogInformation("Starting job host...");

                try
                {
                    var configuration = GetJobHostConfiguration(services);

                    var host = new JobHost(configuration);
                    host.RunAndBlock();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error when starting host");
                    throw;
                }
            }
        }

        private static JobHostConfiguration GetJobHostConfiguration(IServiceProvider services)
        {
            var configuration = new JobHostConfiguration();

            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.BatchSize = 1;
            configuration.Queues.MaxDequeueCount = 10;
            configuration.Queues.VisibilityTimeout = TimeSpan.FromSeconds(30);
            configuration.JobActivator = new ContainerJobActivator(services);
            configuration.LoggerFactory = services.GetService<ILoggerFactory>();

            var webJobOptions = services.GetService<IOptions<AzureWebJobOptions>>().Value;

            configuration.DashboardConnectionString = webJobOptions.DashboardConnectionString;
            configuration.StorageConnectionString = webJobOptions.StorageConnectionString;

            configuration.Tracing.ConsoleLevel = TraceLevel.Off;

            var serviceBusConfiguration = new ServiceBusConfiguration
            {
                ConnectionString = webJobOptions.ServiceBusConnectionString
            };

            configuration.UseServiceBus(serviceBusConfiguration);

            return configuration;
        }
    }
}
