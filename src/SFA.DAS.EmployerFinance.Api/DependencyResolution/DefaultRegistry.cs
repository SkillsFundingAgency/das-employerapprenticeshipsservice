using SFA.DAS.Authorization;
using SFA.DAS.Authorization.WebApi;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.DependencyResolution
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
            For<EmployerFinanceDbContextReadOnly>().Use(c => new EmployerFinanceDbContextReadOnly(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));

            For<IAuthorizationContextCache>().Use<AuthorizationContextCache>();
            For<ICallerContextProvider>().Use<CallerContextProvider>();
        }
    }
}