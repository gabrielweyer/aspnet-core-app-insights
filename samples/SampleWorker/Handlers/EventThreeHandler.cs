using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SampleCore.Events;
using Serilog.Context;

namespace SampleWorker.Handlers
{
    public class EventThreeHandler
    {
        private readonly ILogger<EventThreeHandler> _logger;

        public EventThreeHandler(ILogger<EventThreeHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleEventThreeAsync([ServiceBusTrigger("sync", "event-three")] Message message, CancellationToken token)
        {
            using (LogContext.PushProperty("CorrelationId", message.CorrelationId))
            {
                EventThree eventThree;

                using (var stream = new MemoryStream(message.Body))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    eventThree = JsonConvert.DeserializeObject<EventThree>(reader.ReadToEnd());
                }

                _logger.LogDebug("Processing {@EventThree}", eventThree);

                await Task.CompletedTask;
            }
        }
    }
}
