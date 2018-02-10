using Microsoft.AspNetCore.Builder;
using SimpleInstrumentation.Middlewares;

namespace SimpleInstrumentation.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUserEnricher(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserEnricherMiddleware>();
        }

        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
