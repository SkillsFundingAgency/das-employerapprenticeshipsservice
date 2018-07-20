using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using StructureMap;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class EmployerFinanceDbContextRegistry : Registry
    {
        public EmployerFinanceDbContextRegistry()
        { 
            For<EmployerFinanceDbContext>().Use(c => new EmployerFinanceDbContext(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
        }
    }
}
