﻿using System;
using Docker.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleAspNetCoreAppInsights.Extensions;

namespace Docker.Web
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
            _logger.LogDebug("Configuring services");

            services
                .AddConfigurableAspNetCoreApplicationInsights(_logger, _configuration, "LocalUpstream", typeof(Startup))
                .AddHttpClient("downstream",
                    client => client.BaseAddress = new Uri(_configuration["DownstreamService:BaseAddress"]));

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app)
        {
            _logger.LogDebug("Configuring request pipeline");

            app.UseRequestLogging()
                .UseMvc();
        }
    }
}