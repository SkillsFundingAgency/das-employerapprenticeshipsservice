using System.IO;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class ConfigurationExtensions
{
    public static bool UseGovUkSignIn(this IConfiguration configuration)
    {
        return configuration["SFA.DAS.EmployerAccounts:UseGovSignIn"] != null &&
               configuration["SFA.DAS.EmployerAccounts:UseGovSignIn"]
                  .Equals("true", StringComparison.CurrentCultureIgnoreCase);
    }

    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
        if (!configuration.IsDev())
        {
            configurationBuilder.AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif

        configurationBuilder.AddEnvironmentVariables();

        configurationBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = true;
                options.ConfigurationKeysRawJsonResult = new[] { ConfigurationKeys.EncodingConfig };
            }
        );
        
        return configurationBuilder.Build();
    }
}