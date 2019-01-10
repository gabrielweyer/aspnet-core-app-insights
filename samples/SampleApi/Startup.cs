using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleApi.Extensions;
using SimpleAspNetCoreAppInsights.Extensions;
using SimpleInstrumentation.Extensions;

namespace SampleApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddConfigurableAspNetCoreApplicationInsights(_logger, _configuration, "AppServiceWebApp", typeof(Startup))
                .AddTelemetry()
                .AddInstrumentation()
                .AddAuthentication(_configuration)
                .AddLogic()
                .AddServicebus(_configuration)
                .ConfigureOptions(_configuration)
                .AddMvcCore(config => { config.Filters.Add(new AuthorizeFilter(GetJwtAuthenticatedPolicy())); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonFormatters()
                .AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCorrelation();
            app.UseAuthentication();
            app.UseUserEnricher();
            app.UseMvc();
        }

        private static AuthorizationPolicy GetJwtAuthenticatedPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();
        }
    }
}
