using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace SFA.DAS.EAS.Portal.Startup
{
    public static class LoggingStartup
    {
        public static IHostBuilder ConfigureDasLogging(this IHostBuilder builder)
        {
            return builder.ConfigureLogging(b => b.AddNLog());
        }
    }
}