using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder ConfigureDasAppConfiguration(this IWebHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration(c => c
            .AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = new[]
                {
                    ConfigurationKeys.EmployerApprenticeshipsService
                };
                options.PreFixConfigurationKeys = false;
            })
            .AddAzureTableStorage("SFA.DAS.Support.EAS"));
    }
}
