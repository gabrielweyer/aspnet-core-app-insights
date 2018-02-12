using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleInstrumentation.Models;

namespace SampleApi.Extensions
{
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Turns DeveloperMode on and disable adaptive sampling
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="instrumentationKey"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseDeveloperApplicationInsights(this IWebHostBuilder builder, string instrumentationKey)
        {
            builder.ConfigureServices(services =>
            {
                services.AddApplicationInsightsTelemetry();
                // Based on: https://github.com/Microsoft/ApplicationInsights-aspnetcore/blob/0edd28dad8529546ce337629f05f0d7383a5f489/src/Microsoft.ApplicationInsights.AspNetCore/Extensions/ApplicationInsightsServiceOptions.cs#L14-L21
                services.Configure<ApplicationInsightsServiceOptions>(o =>
                {
                    o.ApplicationVersion = Application.GetAssemblyInformationalversion(typeof(Startup));
                    o.DeveloperMode = true;
                    o.EnableAdaptiveSampling = false;
                    o.InstrumentationKey = instrumentationKey;
                });
            });

            return builder;
        }
    }
}
