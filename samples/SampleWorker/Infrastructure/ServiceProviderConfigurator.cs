using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleWorker.Extensions;
using Serilog;

namespace SampleWorker.Infrastructure
{
    public class ServiceProviderConfigurator
    {
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            return services.BuildServiceProvider();
        }


    }
}
