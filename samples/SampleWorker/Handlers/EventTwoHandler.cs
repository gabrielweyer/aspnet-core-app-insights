using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SampleCore.Events;

namespace SampleWorker.Handlers
{
    public class EventTwoHandler
    {
        private readonly ILogger<EventTwoHandler> _logger;

        public EventTwoHandler(ILogger<EventTwoHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleEventTwoAsync([QueueTrigger("event-two")] EventTwo @event, CancellationToken token)
        {
            _logger.LogDebug("Processing {@EventTwo}", @event);

            await Task.CompletedTask;
        }
    }
}
