using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.ReadStore.ServiceRegistrations;

public static class ReadStoreServiceRegistrations
{
    public static IServiceCollection AddReadStore(this IServiceCollection services)
    {
        services.AddTransient<IDocumentClientFactory, DocumentClientFactory>();

        services.AddSingleton(provider =>
        {
            var clientFactory = provider.GetService<IDocumentClientFactory>();
            return clientFactory.CreateDocumentClient();
        });

        services.AddTransient<IAccountUsersRepository, AccountUsersRepository>();

        return services;
    }
}