using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Jobs.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<EmployerFinanceDbContext>().Use(c => new EmployerFinanceDbContext(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));
        }
    }
}