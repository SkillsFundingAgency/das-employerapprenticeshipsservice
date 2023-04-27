using System.IO;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EAS.Support.Web.Extensions;

public static class ConfigurationExtensions
{
    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
        if (!configuration.IsDev())
        {
            config.AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif

        config.AddEnvironmentVariables();


        config.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
            options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
            options.ConfigurationKeysRawJsonResult = new[]
            {
                    "SFA.DAS.Support.EAS", "SFA.DAS.EmployerAccountAPI", "SFA.DAS.TokenServiceApiClient", "SFA.DAS.EmployerAccounts.Api.Client" 
            };
        });

        return config.Build();
    }
    public static bool IsDev(this IConfiguration configuration)
    {
        return configuration["EnvironmentName"].Equals("Development", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsLocal(this IConfiguration configuration)
    {
        return configuration["EnvironmentName"].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsDevOrLocal(this IConfiguration configuration)
    {
        return IsDev(configuration) || IsLocal(configuration);
    }
}
