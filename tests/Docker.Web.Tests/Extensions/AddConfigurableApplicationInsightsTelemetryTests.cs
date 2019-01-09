using System.Collections.Generic;
using Docker.Web.Extensions;
using FluentAssertions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Docker.Web.Tests.Extensions
{
    public class AddConfigurableApplicationInsightsTelemetryTests
    {
        [Fact]
        public void GivenCustomConfiguration_WhenUsingConfigurableAppInsights_ThenUseCustomConfiguration()
        {
            // Arrange

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ApplicationInsights:ApplicationVersion", "test-local"},
                    {"ApplicationInsights:EnableAdaptiveSampling", "False"},
                    {"ApplicationInsights:TelemetryChannel:DeveloperMode", "true"},
                    {"ApplicationInsights:InstrumentationKey", "instrumentation-key"}
                })
                .Build();

            // Act

            var webHost = WebHost
                .CreateDefaultBuilder()
                .UseStartup<ConfigurableStartup>()
                .UseConfiguration(configuration)
                .Build();

            // Assert

            var configuredOptions = webHost.Services.GetService<IOptions<ApplicationInsightsServiceOptions>>();
            Assert.NotNull(configuredOptions.Value);

            var expectedOptions = new ApplicationInsightsServiceOptions
            {
                EnableAdaptiveSampling = false,
                ApplicationVersion = "test-local",
                DeveloperMode = true,
                InstrumentationKey = "instrumentation-key"
            };

            configuredOptions.Value.Should().BeEquivalentTo(expectedOptions);
        }

        [Fact]
        public void GivenNoCustomConfiguration_WhenUsingConfigurableAppInsights_ThenUseDefaultConfiguration()
        {
            // Arrange

            var configuration = new ConfigurationBuilder().Build();

            // Act

            var webHost = WebHost
                .CreateDefaultBuilder()
                .UseStartup<ConfigurableStartup>()
                .UseConfiguration(configuration)
                .Build();

            // Assert

            var configuredOptions = webHost.Services.GetService<IOptions<ApplicationInsightsServiceOptions>>();
            Assert.NotNull(configuredOptions.Value);
            var actualOptions = configuredOptions.Value;

            Assert.True(actualOptions.EnableAdaptiveSampling);
            Assert.NotEqual("test-local", actualOptions.ApplicationVersion);
            Assert.Null(actualOptions.DeveloperMode);
            Assert.NotEqual("instrumentation-key", actualOptions.InstrumentationKey);
        }

        class ConfigurableStartup
        {
            private readonly ILogger<ConfigurableStartup> _logger;
            private readonly IConfiguration _configuration;

            public ConfigurableStartup(ILogger<ConfigurableStartup> logger, IConfiguration configuration)
            {
                _logger = logger;
                _configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddConfigurableApplicationInsightsTelemetry(_logger, _configuration, "test");
            }

            public void Configure(IApplicationBuilder app)
            {
            }
        }
    }
}