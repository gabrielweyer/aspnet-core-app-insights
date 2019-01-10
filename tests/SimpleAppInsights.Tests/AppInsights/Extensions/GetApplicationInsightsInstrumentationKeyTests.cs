using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SimpleAppInsights.Extensions;
using Xunit;

namespace SimpleAppInsights.Tests.AppInsights.Extensions
{
    public class GetApplicationInsightsInstrumentationKeyTests
    {
        [Fact]
        public void GivenInstrumentationKeySetInNormalConfigurationKey_ThenReturnInstrumentationKey()
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

            var actualInstrumentationKey = configuration.GetApplicationInsightsInstrumentationKey();

            // Assert

            Assert.Equal(expectedInstrumentationKey, actualInstrumentationKey);
        }

        [Fact]
        public void GivenInstrumentationKeyNotSet_ThenReturnNull()
        {
            // Arrange

            var configuration = new ConfigurationBuilder().Build();

            // Act

            var actualInstrumentationKey = configuration.GetApplicationInsightsInstrumentationKey();

            // Assert

            Assert.Null(actualInstrumentationKey);
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

            var actualInstrumentationKey = configuration.GetApplicationInsightsInstrumentationKey();

            // Assert

            Assert.Null(actualInstrumentationKey);
        }
    }
}