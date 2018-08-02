using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution
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

            For<EmployerAccountsDbContext>().Use(c => new EmployerAccountsDbContext(c.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString));
        }
    }
}