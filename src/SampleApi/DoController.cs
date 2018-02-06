using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleApi.Services;

namespace SampleApi
{
    [Route("do")]
    public class DoController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<DoController> _logger;

        public DoController(ITokenService tokenService, ILogger<DoController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("verbose")]
        public void Verbose()
        {
            _logger.LogTrace("It's me logging a VERBOSE event");
        }

        [AllowAnonymous]
        [HttpGet("get-token/{userId:int}")]
        public IActionResult GetToken(int userId)
        {
            var token = _tokenService.GenerateBearerToken(userId);

            return Ok(new {Token = token});
        }
    }
}
