using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using SimpleInstrumentation.Models;

namespace SimpleAppInsights.Telemetry
{
    public class ApplicationInitializer : ITelemetryInitializer
    {
        private readonly ApplicationDescriptor _applicationDescriptor;

        public ApplicationInitializer(ApplicationDescriptor applicationDescriptor)
        {
            _applicationDescriptor = applicationDescriptor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _applicationDescriptor.Name;
            telemetry.Context.Cloud.RoleInstance = _applicationDescriptor.Instance;
            telemetry.Context.Component.Version = _applicationDescriptor.Version;
        }
    }
}
