using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Configuration;

namespace SFA.DAS.EAS.Startup
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