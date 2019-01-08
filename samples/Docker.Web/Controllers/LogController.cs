using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Docker.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpGet("trace")]
        public IActionResult Trace()
        {
            _logger.LogTrace("I'm a trace {OperationIdFromLog}", System.Diagnostics.Activity.Current?.RootId);
            return Accepted();
        }

        [HttpGet("debug")]
        public IActionResult Debug()
        {
            _logger.LogDebug("I'm a debug {OperationIdFromLog}", System.Diagnostics.Activity.Current?.RootId);
            return Accepted();
        }

        [HttpGet("information")]
        public IActionResult Information()
        {
            _logger.LogInformation("I'm an information {OperationIdFromLog}", System.Diagnostics.Activity.Current?.RootId);
            return Accepted();
        }

        [HttpGet("warning")]
        public IActionResult Warning()
        {
            _logger.LogWarning("I'm a warning {OperationIdFromLog}", System.Diagnostics.Activity.Current?.RootId);
            return Accepted();
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            _logger.LogError("I'm an error {OperationIdFromLog}", System.Diagnostics.Activity.Current?.RootId);
            return Accepted();
        }

        [HttpGet("critical")]
        public IActionResult Critical()
        {
            _logger.LogCritical("I'm a critical {OperationIdFromLog}", System.Diagnostics.Activity.Current?.RootId);
            return Accepted();
        }

        [HttpGet("throw")]
        public IActionResult Throw()
        {
            throw new InvalidOperationException("I'm throwing!");
        }
    }
}