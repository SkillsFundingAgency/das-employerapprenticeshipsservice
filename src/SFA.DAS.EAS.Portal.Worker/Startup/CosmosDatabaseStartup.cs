using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Client.Configuration;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Database;

namespace SFA.DAS.EAS.Portal.Worker.Startup
{
    public static class CosmosDatabaseStartup
    {
        public static IServiceCollection AddCosmosDatabase(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            return services
                .AddTransient<IDocumentClientFactory, DocumentClientFactory>()
                .AddSingleton(sp => sp.GetService<IDocumentClientFactory>().CreateDocumentClient())
                .AddSingleton<IAccountsRepository, AccountsRepository>()
                .Configure<CosmosDatabaseConfiguration>(configuration.GetPortalSection("CosmosDatabase"));
        }
    }
}
