using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.Infrastructure.Configuration;

namespace SFA.DAS.EAS.Portal.Worker.Startup
{
    public static class CosmosDatabaseStartup
    {
        public static IServiceCollection AddCosmosDatabase(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            return services
                .AddTransient<IDocumentClientFactory, DocumentClientFactory>()
                .AddSingleton(sp => sp.GetService<IDocumentClientFactory>().CreateDocumentClient())
                .AddSingleton<IAccountsRepository, AccountsRepository>()
                .Configure<CosmosDatabaseConfiguration>(hostBuilderContext.Configuration.GetPortalSection(PortalSections.CosmosDatabase));
        }
    }
}
