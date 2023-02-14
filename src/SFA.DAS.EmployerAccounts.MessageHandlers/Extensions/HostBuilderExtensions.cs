using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseStructureMap(this IHostBuilder builder)
    {
        return UseStructureMap(builder, registry: null);
    }

    public static IHostBuilder UseStructureMap(this IHostBuilder builder, Registry registry)
    {
        return builder.UseServiceProviderFactory(new StructureMapServiceProviderFactory(registry));
    }

    public static IHostBuilder ConfigureDasAppConfiguration(this IHostBuilder hostBuilder, string[] args)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddAzureTableStorage(ConfigurationKeys.EmployerAccounts)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
        });
    }
}