using SimpleAppInsights.Options;

namespace SimpleAspNetCoreAppInsights.Options
{
    public class ApplicationInsightsAspNetCoreSimpleOptions
    {
        public string ApplicationVersion { get; set; }
        public string InstrumentationKey { get; set; }
        public TelemetryChannelSimpleOptions TelemetryChannel { get; set; }
        public bool EnableAdaptiveSampling { get; set; } = true;

        private ApplicationInsightsAspNetCoreSimpleOptions(ApplicationInsightsSimpleOptions o)
        {
            ApplicationVersion = o.ApplicationVersion;
            InstrumentationKey = o.InstrumentationKey;
            TelemetryChannel = o.TelemetryChannel;
        }

        public static implicit operator ApplicationInsightsAspNetCoreSimpleOptions(ApplicationInsightsSimpleOptions o)
        {
            return o == null ? null : new ApplicationInsightsAspNetCoreSimpleOptions(o);
        }
    }
}