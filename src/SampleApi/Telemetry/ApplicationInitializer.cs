using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using SampleApi.Models;

namespace SampleApi.Telemetry
{
    public class ApplicationInitializer : ITelemetryInitializer
    {
        private readonly Application _application;

        public ApplicationInitializer(Application application)
        {
            _application = application;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Component.Version = _application.Build;
            telemetry.Context.Properties["Application Name"] = _application.Name;
            telemetry.Context.Properties["Host"] = _application.Host;
        }
    }
}
