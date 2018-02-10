using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace SimpleInstrumentation.Middlewares
{
    public class UserEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public UserEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string userId = null;

            if (context.User.Identity.IsAuthenticated)
            {
                userId = context.User.FindFirstValue(ClaimTypes.Name);
            }

            using (LogContext.PushProperty("UserId", userId))
            {
                await _next(context);
            }
        }
    }
}
