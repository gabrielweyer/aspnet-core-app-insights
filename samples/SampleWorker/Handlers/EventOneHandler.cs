using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SampleCore.Events;

namespace SampleWorker.Handlers
{
    public class EventOneHandler
    {
        private readonly ILogger<EventOneHandler> _logger;

        public EventOneHandler(ILogger<EventOneHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleEventOneAsync([ServiceBusTrigger("event-one")] EventOne @event, CancellationToken token)
        {
            _logger.LogDebug("Processing {@EventOne}", @event);

            await Task.CompletedTask;
        }
    }
}
