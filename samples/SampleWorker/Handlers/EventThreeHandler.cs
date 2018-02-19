using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
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

        public async Task HandleEventThreeAsync([ServiceBusTrigger("sync", "event-three", AccessRights.Listen)] BrokeredMessage message, CancellationToken token)
        {
            using (LogContext.PushProperty("CorrelationId", message.CorrelationId))
            {
                EventThree eventThree = null;

                using (var stream = message.GetBody<Stream>())
                using (var reader = new StreamReader(stream))
                {
                    eventThree = JsonConvert.DeserializeObject<EventThree>(reader.ReadToEnd());
                }

                _logger.LogDebug("Processing {@EventThree}", eventThree);

                await Task.CompletedTask;
            }
        }
    }
}
