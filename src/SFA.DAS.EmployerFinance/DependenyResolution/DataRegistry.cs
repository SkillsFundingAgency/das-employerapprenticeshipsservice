using SFA.DAS.EmployerFinance.Configuration;
using StructureMap;
using System.Data.Common;
using System.Data.SqlClient;

namespace SFA.DAS.EmployerFinance.DependenyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));
            //ForConcreteType<EmployerFinanceDbContext>();
        }
    }
}
