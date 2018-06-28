using System.Data.Common;
using System.Data.SqlClient;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            ForConcreteType<EmployerAccountDbContext>();
            ForConcreteType<EmployerFinancialDbContext>();
        }
    }
}