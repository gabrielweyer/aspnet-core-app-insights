using Microsoft.ApplicationInsights.DataContracts;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;
using Xunit;

namespace SimpleAppInsights.Tests.AppInsights.Telemetry
{
    public class ApplicationInitializerTests
    {
        [Fact]
        public void GivenApplicationDescriptor_ThenSetTelemetryItem()
        {
            // Arrange

            var applicationDescriptor = new ApplicationDescriptor("application-name", "20190110.03");
            var applicationInitializer = new ApplicationInitializer(applicationDescriptor);

            var telemetryItem = new TraceTelemetry();

            // Act

            applicationInitializer.Initialize(telemetryItem);

            // Assert

            Assert.Equal(applicationDescriptor.Name, telemetryItem.Context.Cloud.RoleName);
            Assert.Equal(applicationDescriptor.Instance, telemetryItem.Context.Cloud.RoleInstance);
            Assert.Equal(applicationDescriptor.Version, telemetryItem.Context.Component.Version);
        }
    }
}