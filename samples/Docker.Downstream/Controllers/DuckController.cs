using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Docker.Downstream.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DuckController : ControllerBase
    {
        private readonly ILogger<DuckController> _logger;

        private static readonly Random SomewhatRandom = new Random();

        public DuckController(ILogger<DuckController> logger)
        {
            _logger = logger;
        }

        [Route("")]
        [Produces("application/json")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Getting the duck to quack");

            ThrowAnExceptionEveryNowAndThen();

            await DoTheHeavyLiftingAsync();

            return Ok("Quack");
        }

        private void ThrowAnExceptionEveryNowAndThen()
        {
            var someValue = SomewhatRandom.Next(9);

            if (someValue > 2) return;

            _logger.LogDebug("Sorry fella, we'll now proceed to throw an Exception for science");

            throw new InvalidOperationException("I felt like throwing an Exception.");
        }

        private Task DoTheHeavyLiftingAsync()
        {
            var delayMilliseconds = SomewhatRandom.Next(200, 301);

            _logger.LogDebug("Looks like we will delay the quacking by {DuckDelayMs} ms", delayMilliseconds);

            return Task.Delay(delayMilliseconds);
        }
    }
}