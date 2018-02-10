using System.Text;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SampleApi.Options;
using SampleApi.Services;
using SimpleAppInsights.Telemetry;
using SimpleInstrumentation.Models;

namespace SampleApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
        }

        public static void AddAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
        {
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
        }

        public static void AddTelemetry(this IServiceCollection services, string applicationName)
        {
            services.AddSingleton(new Application(applicationName, Application.GetAssemblyInformationalversion(typeof(Startup))));
            services.AddSingleton<ITelemetryInitializer, AuthenticatedUserInitializer>();
            services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();
        }
    }
}
