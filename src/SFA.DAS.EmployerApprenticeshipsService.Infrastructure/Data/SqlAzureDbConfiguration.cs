using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class SqlAzureDbConfiguration : DbConfiguration
    {
        public SqlAzureDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}