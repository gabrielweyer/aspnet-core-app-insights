using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace SampleApi.Services
{
    public class AzureTopicClient : IAzureTopicClient
    {
        private readonly TopicClient _topicClient;
        private readonly ICorrelator _correlator;

        public AzureTopicClient(TopicClient topicClient, ICorrelator correlator)
        {
            _topicClient = topicClient;
            _correlator = correlator;
        }

        public async Task SendAsync<T>(T model)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
            var message = new Message(bytes)
            {
                ContentType = "application/json",
                CorrelationId = _correlator.CorrelationId
            };

            await _topicClient.SendAsync(message);
        }
    }

    public interface IAzureTopicClient
    {
        Task SendAsync<T>(T model);
    }
}
