using Microsoft.Azure.Documents;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClientRegistry : Registry
    {
        public EmployerAccountsApiClientRegistry()
        {
            For<IEmployerAccountsApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerAccountsApiClientConfiguration>("SFA.DAS.EmployerAccounts.Api.Client")).Singleton();
            For<ReadStoreServiceFactory>().Use<ReadStoreServiceFactory>(c => c.GetInstance);
            For<IReadStoreMediator>().Use<ReadStoreMediator>();
            For<IReadStoreRequestHandler<HasRoleQuery, bool>>().Use<HasRoleQueryHandler>();
            For<IDocumentClient>().Add(c => c.GetInstance<IDocumentClientFactory>().CreateDocumentClient())
                .Named(GetType().FullName).Singleton();
            For<IDocumentClientFactory>().Use<DocumentClientFactory>();
            For<IAccountUsersRepository>().Use<AccountUsersRepository>().Ctor<IDocumentClient>().IsNamedInstance(GetType().FullName);
            For<IEmployerAccountsApiClient>().Use<EmployerAccountsApiClient>();
        }
    }
}