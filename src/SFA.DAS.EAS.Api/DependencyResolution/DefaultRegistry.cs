using System.Data.Common;
using System.Data.SqlClient;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.WebApi;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.UnitOfWork;
using StructureMap;

namespace SFA.DAS.EAS.Account.Api.DependencyResolution
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

            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            For<EmployerAccountsDbContext>().Use(c => GetAcccountsDbContext(c));
            For<EmployerFinanceDbContext>().Use(c => GetFinanceDbContext(c));
            For<IAuthorizationContextCache>().Use<AuthorizationContextCache>();
            For<ICallerContextProvider>().Use<CallerContextProvider>();
        }

        private EmployerAccountsDbContext GetAcccountsDbContext(IContext context)
        {
            var db = new EmployerAccountsDbContext(context.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString);
            db.Database.BeginTransaction();
            return db;
        }

        private EmployerFinanceDbContext GetFinanceDbContext(IContext context)
        {
            var db = new EmployerFinanceDbContext(context.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString);
            db.Database.BeginTransaction();
            return db;
        }

    }
}