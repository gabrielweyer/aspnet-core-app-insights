using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Docker.Downstream.Middleware
{
    /// <summary>
    /// Inspired by https://github.com/datalust/serilog-middleware-example/blob/master/src/Datalust.SerilogMiddlewareExample/Diagnostics/SerilogMiddleware.cs
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<RequestLoggingMiddleware> logger)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var start = Stopwatch.GetTimestamp();

            try
            {
                await _next(httpContext);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                var statusCode = httpContext.Response.StatusCode;
                var level = statusCode > 499 ? LogLevel.Error : LogLevel.Information;

                logger.Log(level, MessageTemplate, httpContext.Request.Method, GetPath(httpContext), statusCode,
                    elapsedMs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, MessageTemplate, httpContext.Request.Method, GetPath(httpContext), 500, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()));

                httpContext.Response.StatusCode = 500;
            }
        }

        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double) Stopwatch.Frequency;
        }

        private static string GetPath(HttpContext httpContext)
        {
            return httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? httpContext.Request.Path.ToString();
        }
    }
}