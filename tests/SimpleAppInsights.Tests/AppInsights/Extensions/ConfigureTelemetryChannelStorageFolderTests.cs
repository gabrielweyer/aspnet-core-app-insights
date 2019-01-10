using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleAppInsights.Extensions;
using Xunit;

namespace SimpleAppInsights.Tests.AppInsights.Extensions
{
    public class ConfigureTelemetryChannelStorageFolderTests
    {
        private readonly ILogger<ConfigureTelemetryChannelStorageFolderTests> _logger =
            new NullLogger<ConfigureTelemetryChannelStorageFolderTests>();

        [Fact]
        public void GivenWindowsAndGivenStorageFolderConfiguration_ThenDoNotRegisterTelemetryChannel()
        {
            // Arrange

            var services = new ServiceCollection();

            // Act

            services.ConfigureTelemetryChannelStorageFolder(_logger, "/dev/null", true);

            // Assert

            var serviceProvider = services.BuildServiceProvider();
            var telemetryChannel = serviceProvider.GetService<ITelemetryChannel>();
            Assert.Null(telemetryChannel);
        }

        [Fact]
        public void GiveUnixAndGivenStorageFolderConfiguration_ThenRegisterTelemetryChannel()
        {
            // Arrange

            var services = new ServiceCollection();

            // Act

            services.ConfigureTelemetryChannelStorageFolder(_logger, "/dev/null", false);

            // Assert

            var serviceProvider = services.BuildServiceProvider();
            var telemetryChannel = serviceProvider.GetService<ITelemetryChannel>();
            Assert.NotNull(telemetryChannel);
        }
        
        [Fact]
        public void GiveUnixAndGivenNoStorageFolderConfiguration_ThenDoNotRegisterTelemetryChannel()
        {
            // Arrange

            var services = new ServiceCollection();

            // Act

            services.ConfigureTelemetryChannelStorageFolder(_logger, null, false);

            // Assert

            var serviceProvider = services.BuildServiceProvider();
            var telemetryChannel = serviceProvider.GetService<ITelemetryChannel>();
            Assert.Null(telemetryChannel);
        }
    }
}