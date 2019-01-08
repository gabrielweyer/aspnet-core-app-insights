using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Docker.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownstreamController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DownstreamController> _logger;

        public DownstreamController(IHttpClientFactory httpClientFactory, ILogger<DownstreamController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("duck")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTheDuck()
        {
            var httpClient = _httpClientFactory.CreateClient("downstream");

            var duckResponse = await httpClient.GetAsync("duck");

            duckResponse.EnsureSuccessStatusCode();

            _logger.LogInformation("We got a duck {DuckLength}", duckResponse.Content.Headers.ContentLength);

            return Ok(await duckResponse.Content.ReadAsAsync<string>());
        }
    }
}