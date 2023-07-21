using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions
{
    public class RobotsTextMiddleware
    {
        private readonly RequestDelegate _next;
        public RobotsTextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/robots.txt")
            {
                context.Response.ContentType = "text/plain";
                await context.Response.SendFileAsync("robots.txt");
            }
            else
            {
                await _next(context);
            }

        }
    }
}
