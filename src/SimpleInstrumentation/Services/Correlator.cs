namespace SampleApi.Services
{
    public class Correlator : ICorrelator
    {
        public string HeaderName { get; }
        public string CorrelationId { get; set; }

        public Correlator()
        {
            HeaderName = "X-Correlation-Id";
        }
    }

    public interface ICorrelator
    {
        string HeaderName { get; }
        string CorrelationId { get; set; }
    }
}
