using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SampleApi.Services;
using Serilog.Context;

namespace SimpleInstrumentation.Middlewares
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ICorrelator correlator, ILogger<CorrelationMiddleware> logger)
        {
            if (context.Request.Headers.TryGetValue(correlator.HeaderName, out var correlationId))
            {
                logger.LogDebug("Read CorrelationId from header");

                context.TraceIdentifier = correlationId;
            }
            else
            {
                correlationId = context.TraceIdentifier;
            }

            correlator.CorrelationId = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(correlator.HeaderName, new[] { context.TraceIdentifier });
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
