using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SimpleAppInsights.Extensions;
using Xunit;

namespace SimpleAppInsights.Tests.AppInsights.Extensions
{
    public class GetApplicationInsightsOptionsTests
    {
        [Fact]
        public void GivenApplicationInsightsApplicationVersionSetInNormalConfigurationKey_ThenUseApplicationInsightsApplicationVersion()
        {
            // Arrange

            const string expectedApplicationVersion = "5.5.3-alpha";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApplicationInsights:ApplicationVersion", expectedApplicationVersion }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.Equal(expectedApplicationVersion, actualOptions.ApplicationVersion);
        }

        [Fact]
        public void GivenApplicationInsightsApplicationVersionSetInKnownConfigurationKey_ThenIgnoreItAndUseAssemblyInformationalVersion()
        {
            // Arrange

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "version", "5.5.3-alpha" }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            const string defaultAssemblyInformationalVersion = "1.0.0";
            Assert.Equal(defaultAssemblyInformationalVersion, actualOptions.ApplicationVersion);
        }

        [Fact]
        public void GivenApplicationInsightsApplicationVersionNotSet_ThenUseAssemblyInformationalVersion()
        {
            // Arrange

            var configuration = new ConfigurationBuilder().Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            const string defaultAssemblyInformationalVersion = "1.0.0";
            Assert.Equal(defaultAssemblyInformationalVersion, actualOptions.ApplicationVersion);
        }

        [Fact]
        public void GivenInstrumentationKeySetInNormalConfigurationKey_ThenSetInstrumentationKey()
        {
            // Arrange

            const string expectedInstrumentationKey = "key";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApplicationInsights:InstrumentationKey", expectedInstrumentationKey }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.Equal(expectedInstrumentationKey, actualOptions.InstrumentationKey);
        }

        [Fact]
        public void GivenInstrumentationKeyNotSet_ThenInstrumentationKeyIsNull()
        {
            // Arrange

            var configuration = new ConfigurationBuilder().Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.Null(actualOptions.InstrumentationKey);
        }

        [Fact]
        public void GivenInstrumentationKeySetInKnownConfiguration_ThenInstrumentationKeyIsIgnored()
        {
            // Arrange

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "APPINSIGHTS_INSTRUMENTATIONKEY", "key" }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.Null(actualOptions.InstrumentationKey);
        }

        [Fact]
        public void GivenDeveloperModeNotSet_ThenDeveloperModeIsFalse()
        {
            // Arrange

            var configuration = new ConfigurationBuilder().Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.NotNull(actualOptions.TelemetryChannel);
            Assert.False(actualOptions.TelemetryChannel.DeveloperMode);
        }

        [Fact]
        public void GivenDeveloperModeSetAsTrueInKnownConfigurationKey_ThenDeveloperModeIsFalse()
        {
            // Arrange

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "APPINSIGHTS_DEVELOPER_MODE", "true" }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.NotNull(actualOptions.TelemetryChannel);
            Assert.False(actualOptions.TelemetryChannel.DeveloperMode);
        }

        [Fact]
        public void GivenDeveloperModeSetAsTrueInNormalConfigurationKey_ThenDeveloperModeIsTrue()
        {
            // Arrange

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApplicationInsights:TelemetryChannel:DeveloperMode", "true" }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.NotNull(actualOptions.TelemetryChannel);
            Assert.True(actualOptions.TelemetryChannel.DeveloperMode);
        }

        [Fact]
        public void GivenStorageFolderSetInNormalConfigurationKey_ThenSetItInOptions()
        {
            // Arrange

            const string expectedStorageFolder = "/dev/null";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApplicationInsights:TelemetryChannel:StorageFolder", expectedStorageFolder }
                })
                .Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.NotNull(actualOptions.TelemetryChannel);
            Assert.Equal(expectedStorageFolder, actualOptions.TelemetryChannel.StorageFolder);
        }

        [Fact]
        public void GivenStorageFolderNotSet_ThenStorageFolderIsNullInOptions()
        {
            // Arrange

            var configuration = new ConfigurationBuilder().Build();

            // Act

            var actualOptions = configuration.GetApplicationInsightsOptions(typeof(GetApplicationInsightsOptionsTests));

            // Assert

            Assert.NotNull(actualOptions);
            Assert.NotNull(actualOptions.TelemetryChannel);
            Assert.Null(actualOptions.TelemetryChannel.StorageFolder);
        }
    }
}