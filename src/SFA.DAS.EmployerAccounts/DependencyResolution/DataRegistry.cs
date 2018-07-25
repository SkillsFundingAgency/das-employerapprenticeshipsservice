using System.Data.Common;
using System.Data.SqlClient;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString));
            ForConcreteType<EmployerAccountsDbContext>();
        }
    }
}
