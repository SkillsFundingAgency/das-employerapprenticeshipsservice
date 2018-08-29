using SFA.DAS.Authorization;
using SFA.DAS.Authorization.WebApi;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using StructureMap;

namespace SFA.DAS.EAS.Account.Api.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<EmployerFinanceDbContext>().Use(c => new EmployerFinanceDbContext(c.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString));
            For<IAuthorizationContextCache>().Use<AuthorizationContextCache>();
        }
    }
}