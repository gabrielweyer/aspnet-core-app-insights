using Microsoft.AspNetCore.Builder;
using SampleApi.Middlewares;

namespace SampleApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUserEnricher(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserEnricherMiddleware>();
        }
    }
}
