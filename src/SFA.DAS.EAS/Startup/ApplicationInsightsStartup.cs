using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Startup
{
    public static class ApplicationInsightsStartup
    {
        public static IHostBuilder UseApplicationInsights(this IHostBuilder builder)
        {
            builder.ConfigureLogging((c, b) => b.AddApplicationInsights(c.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]));

            return builder;
        }
    }
}