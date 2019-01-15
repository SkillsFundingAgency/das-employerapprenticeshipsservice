using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.ReadStore.DependencyResolution
{
    internal class ReadStoreMediatorRegistry : Registry
    {
        public ReadStoreMediatorRegistry()
        {
            For<ReadStoreServiceFactory>().Use<ReadStoreServiceFactory>(c => c.GetInstance);
            For<IReadStoreMediator>().Use<ReadStoreMediator>();
            For<IReadStoreRequestHandler<CreateAccountUserCommand, Unit>>().Use<CreateAccountUserCommandHandler>();
            For<IReadStoreRequestHandler<UpdateAccountUserCommand, Unit>>().Use<UpdateAccountUserCommandHandler>();
            For<IReadStoreRequestHandler<RemoveAccountUserCommand, Unit>>().Use<RemoveAccountUserCommandHandler>();
            For<IReadStoreRequestHandler<IsUserInRoleQuery, bool>>().Use<IsUserInRoleQueryHandler>();
            For<IReadStoreRequestHandler<IsUserInAnyRoleQuery, bool>>().Use<IsUserInAnyRoleQueryHandler>();
        }
    }
}