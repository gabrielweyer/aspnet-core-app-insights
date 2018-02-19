using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleApi.Services;
using SampleCore.Events;

namespace SampleApi
{
    [Route("do")]
    public class DoController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IAzureTopicClient _azureTopicClient;
        private readonly ILogger<DoController> _logger;

        public DoController(ITokenService tokenService, IAzureTopicClient azureTopicClient, ILogger<DoController> logger)
        {
            _tokenService = tokenService;
            _azureTopicClient = azureTopicClient;
            _logger = logger;
        }

        [HttpGet("verbose")]
        public void Verbose()
        {
            _logger.LogTrace("It's me logging a VERBOSE event");
        }

        [HttpGet("debug")]
        public void Debug()
        {
            _logger.LogDebug("It's me logging a DEBUG event");
        }

        [HttpGet("information")]
        public void Information()
        {
            _logger.LogInformation("It's me logging a INFORMATION event");
        }

        [HttpGet("warning")]
        public void Warning()
        {
            _logger.LogInformation("It's me logging a WARNING event");
        }

        [HttpGet("throw")]
        public async Task Throw()
        {
            await UselessAsync();
        }

        [HttpGet("web-job-three")]
        public async Task WebJobThree()
        {
            var eventThree = new EventThree
            {
                Duration = TimeSpan.FromHours(2),
                TemperatureCelsius = 15
            };

            await _azureTopicClient.SendAsync(eventThree);
        }

        [AllowAnonymous]
        [HttpGet("get-token/{userId:int}")]
        public IActionResult GetToken(int userId)
        {
            var token = _tokenService.GenerateBearerToken(userId);

            return Ok(new {Token = token});
        }

        private async Task UselessAsync()
        {
            await MoreUselessAsync();
        }

        private async Task MoreUselessAsync()
        {
            await EvenMoreUselessAsync("hello");
        }

        private async Task EvenMoreUselessAsync(string someArgument)
        {
            await Task.CompletedTask;

            throw new ArgumentOutOfRangeException(nameof(someArgument), someArgument, "That is not a good argument.");
        }
    }
}
