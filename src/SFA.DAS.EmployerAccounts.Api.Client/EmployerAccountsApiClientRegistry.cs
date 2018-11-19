using Microsoft.Azure.Documents;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Queries;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClientRegistry : Registry
    {
        public EmployerAccountsApiClientRegistry()
        {
            For<IEmployerAccountsApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerAccountsApiClientConfiguration>("SFA.DAS.EmployerAccounts.Api.Client")).Singleton();
            For<ApiServiceFactory>().Use<ApiServiceFactory>(c => c.GetInstance);
            For<IApiMediator>().Use<ApiMediator>();
            For<IApiRequestHandler<HasRoleQuery, HasRoleQueryResult>>().Use<HasRoleQueryHandler>();
            For<IDocumentClient>().Add(c => c.GetInstance<IDocumentClientFactory>().CreateDocumentClient())
                .Named(GetType().FullName).Singleton();
            For<IDocumentClientFactory>().Use<DocumentClientFactory>();
            For<IUsersRolesRepository>().Use<UserRolesRepository>().Ctor<IDocumentClient>().IsNamedInstance(GetType().FullName);
            For<IEmployerAccountsApiClient>().Use<EmployerAccountsApiClient>();
        }
    }
}