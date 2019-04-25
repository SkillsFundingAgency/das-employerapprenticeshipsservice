using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Startup
{
    /// <remarks>
    /// Useful link: https://github.com/Azure/azure-webjobs-sdk/wiki/Application-Insights-Integration
    /// </remarks>
    public static class ApplicationInsightsStartup
    {
        public static IHostBuilder UseApplicationInsights(this IHostBuilder builder)
        {
            builder.ConfigureLogging((c, b) => b.AddApplicationInsights(o => o.InstrumentationKey = c.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]));

            return builder;
        }
    }
}