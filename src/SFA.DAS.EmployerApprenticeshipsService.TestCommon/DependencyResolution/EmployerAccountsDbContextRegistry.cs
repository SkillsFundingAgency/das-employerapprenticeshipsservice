using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NServiceBus;
using StructureMap;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class EmployerAccountsDbContextRegistry : Registry
    {
        public EmployerAccountsDbContextRegistry()
        {
            For<EmployerAccountsDbContext>().Use(c => new EmployerAccountsDbContext(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            //For<EmployerAccountsDbContext>().Use(c => new EmployerAccountsDbContext(c.GetInstance<IUnitOfWorkContext>()));
        }
    }
}
