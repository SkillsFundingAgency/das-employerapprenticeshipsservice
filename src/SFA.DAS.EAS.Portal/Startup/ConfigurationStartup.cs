using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EAS.Portal.Startup
{
    public static class ConfigurationStartup
    {
        public static IHostBuilder ConfigureDasAppConfiguration(this IHostBuilder builder, string[] args)
        {
            return builder.ConfigureAppConfiguration((c, b) => b
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{c.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                //todo: we give it a valid config, until we have our new one
                .AddAzureTableStorage("SFA.DAS.EmployerApprenticeshipsService") // ConfigurationKeys.EmployerApprenticeshipsServiceHomepage)
                .AddCommandLine(args));
        }
    }
}