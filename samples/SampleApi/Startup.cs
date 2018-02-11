using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleApi.Extensions;
using SampleApi.Options;
using SimpleInstrumentation.Extensions;

namespace SampleApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTelemetry("Web");
            services.AddInstrumentation();

            var jwtOptions = _configuration.GetSection("Jwt").Get<JwtOptions>();
            services.AddAuthentication(jwtOptions);

            services.AddServices();
            services.ConfigureOptions(_configuration);

            var policy = GetJwtAuthenticatedPolicy();

            services.AddMvcCore(config => { config.Filters.Add(new AuthorizeFilter(policy)); })
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
