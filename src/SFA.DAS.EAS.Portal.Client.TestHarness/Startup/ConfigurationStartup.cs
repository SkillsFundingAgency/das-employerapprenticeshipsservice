using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Portal.Configuration;
using ConfigurationKeys = SFA.DAS.EAS.Portal.Client.Configuration.ConfigurationKeys;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.Startup
{
    public static class ConfigurationStartup
    {
        public static IHostBuilder ConfigurePortalClientConfiguration(this IHostBuilder builder, string[] args)
        {
            return builder.ConfigureAppConfiguration((c, b) => b
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{c.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .AddAzureTableStorage(o =>
                {
                    o.EnvironmentNameEnvironmentVariableName = EnvironmentVariableName.EnvironmentName;
                    o.ConfigurationKeys = new[] {ConfigurationKeys.PortalClient};
                })
                .AddCommandLine(args));
        }
    }
}