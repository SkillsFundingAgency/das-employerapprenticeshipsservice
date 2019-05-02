using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Database;

namespace SFA.DAS.EAS.Portal.Worker.Startup
{
    public static class CosmosDatabaseStartup
    {
        public static IServiceCollection AddCosmosDatabase(this IServiceCollection services)
        {
            //todo: better way to get at config?
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            return services
                .AddTransient<IDocumentClientFactory, DocumentClientFactory>()
                //todo: provider permissions adds this as a singleton! if it should be a singleton, why not just use the factory here and not add the factory??
                .AddTransient(sp => sp.GetService<IDocumentClientFactory>().CreateDocumentClient())
                .AddTransient<IAccountsRepository, AccountsRepository>()
                .Configure<CosmosDatabaseConfiguration>(configuration.GetPortalSection("CosmosDatabase"));
        }
    }
}
