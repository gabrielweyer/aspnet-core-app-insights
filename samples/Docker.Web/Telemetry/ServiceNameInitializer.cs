using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Docker.Web.Telemetry
{
    public class ServiceNameInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = "LoggingWeb";
        }
    }
}