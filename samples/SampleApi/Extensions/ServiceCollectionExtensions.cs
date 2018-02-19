using System.Text;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleApi.Options;
using SampleApi.Services;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;

namespace SampleApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<ServiceBusOptions>(configuration.GetSection("ServiceBus"));

            return services;
        }

        public static IServiceCollection AddLogic(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAzureTopicClient, AzureTopicClient>();

            return services;
        }

        public static IServiceCollection AddServicebus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(p =>
            {
                var serviceBusOptions = p.GetService<IOptions<ServiceBusOptions>>().Value;

                return new TopicClient(
                    serviceBusOptions.ConnectionString,
                    serviceBusOptions.Topic);
            });

            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                });

            return services;
        }

        public static IServiceCollection AddTelemetry(this IServiceCollection services, string applicationName)
        {
            services.AddSingleton(new Application(applicationName, Application.GetAssemblyInformationalversion(typeof(Startup))));
            services.AddSingleton<ITelemetryInitializer, AuthenticatedUserInitializer>();
            services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

            return services;
        }

        public static IServiceCollection AddInstrumentation(this IServiceCollection services)
        {
            services.AddSingleton<ICorrelator, Correlator>();

            return services;
        }
    }
}
