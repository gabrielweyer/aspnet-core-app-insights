namespace SampleWorker.Options
{
    public class AzureWebJobOptions
    {
        public string DashboardConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
