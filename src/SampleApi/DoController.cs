using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleApi
{
    [Route("do")]
    public class DoController : Controller
    {
        private readonly ILogger<DoController> _logger;

        public DoController(ILogger<DoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("verbose")]
        public void Verbose()
        {
            _logger.LogTrace("It's me logging a VERBOSE event");
        }
    }
}
