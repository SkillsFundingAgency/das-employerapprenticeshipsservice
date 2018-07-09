using System.Data.Common;
using System.Data.SqlClient;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString));
        }
    }
}