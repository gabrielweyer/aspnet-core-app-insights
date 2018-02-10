using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace SimpleAppInsights.Telemetry
{
    public class AuthenticatedUserInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedUserInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (_httpContextAccessor.HttpContext == null) return;

            var identity = _httpContextAccessor.HttpContext.User.Identity;

            if (!identity.IsAuthenticated) return;

            telemetry.Context.User.Id = identity.Name;
            telemetry.Context.User.AuthenticatedUserId = identity.Name;
        }
    }
}
