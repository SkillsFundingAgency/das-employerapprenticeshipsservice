using Microsoft.Azure.Documents;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ReadStoreDataServiceRegistrations
{
    public static IServiceCollection AddReadStoreServices(this IServiceCollection services)
    {
        services.AddTransient<IDocumentClientFactory, DocumentClientFactory>();

        services.AddSingleton<IDocumentClient>(provider =>
        {
            var factory = provider.GetService<IDocumentClientFactory>();
            return factory.CreateDocumentClient();
        });

        services.AddTransient<IAccountUsersRepository, AccountUsersRepository>();

        return services;
    }
}