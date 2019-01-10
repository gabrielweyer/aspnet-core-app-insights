using System;
using SimpleInstrumentation.Models;
using Xunit;

namespace SimpleAppInsights.Tests.Instrumentation.Models
{
    public class ApplicationDescriptorTests
    {
        [Fact]
        public void GivenApplicationDescriptorIsConstructed_ThenUseMachineNameAsInstance()
        {
            // Act

            var actualApplicationDescriptor = new ApplicationDescriptor("some-app-name", "20190101.03");

            // Assert

            Assert.Equal(Environment.MachineName, actualApplicationDescriptor.Instance);
        }
    }
}