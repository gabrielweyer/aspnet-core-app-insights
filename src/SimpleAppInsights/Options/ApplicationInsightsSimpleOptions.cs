namespace SimpleAppInsights.Options
{
    public class ApplicationInsightsSimpleOptions
    {
        public string ApplicationVersion { get; set; }
        public string InstrumentationKey { get; set; }
        public TelemetryChannelSimpleOptions TelemetryChannel { get; set; } = new TelemetryChannelSimpleOptions();
    }

    public class TelemetryChannelSimpleOptions
    {
        public bool DeveloperMode { get; set; }
        public string StorageFolder { get; set; }
    }
}