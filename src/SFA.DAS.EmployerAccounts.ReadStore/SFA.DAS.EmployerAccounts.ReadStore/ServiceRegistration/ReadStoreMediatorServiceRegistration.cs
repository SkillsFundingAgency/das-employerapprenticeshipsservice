using Microsoft.Azure.Documents;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.ServiceRegistration;

public static class ReadStoreMediatorServiceRegistration
{
    public static IServiceCollection AddReadstoreMediator(this IServiceCollection services)
    {
        //For<ReadStoreServiceFactory>().Use<ReadStoreServiceFactory>(c => c.GetInstance);

        services.AddTransient<ReadStoreServiceFactory, ReadStoreServiceFactory>();
        services.AddTransient<IReadStoreMediator, ReadStoreMediator>();
        services.AddTransient<IReadStoreRequestHandler<CreateAccountUserCommand, Unit>, CreateAccountUserCommandHandler>();
        services.AddTransient<IReadStoreRequestHandler<IsUserInRoleQuery, bool>, IsUserInRoleQueryHandler>();
        services.AddTransient<IReadStoreRequestHandler<IsUserInAnyRoleQuery, bool>, IsUserInAnyRoleQueryHandler>();
        services.AddTransient<IReadStoreRequestHandler<RemoveAccountUserCommand, Unit>, RemoveAccountUserCommandHandler>();
        services.AddTransient<IReadStoreRequestHandler<PingQuery, Unit>, PingQueryHandler>(x =>
            {
                var documentClient = x.GetService<IDocumentClient>();
                return new PingQueryHandler(documentClient);
            });
        services.AddTransient<IReadStoreRequestHandler<UpdateAccountUserCommand, Unit>, UpdateAccountUserCommandHandler>();

        return services;
    }
}