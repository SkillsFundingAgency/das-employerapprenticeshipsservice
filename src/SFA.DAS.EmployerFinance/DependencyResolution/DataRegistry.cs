using System.Data.Common;
using System.Data.SqlClient;
using NServiceBus.Persistence;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EntityFramework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.UnitOfWork;
using StructureMap;
using IUnitOfWorkManager = SFA.DAS.EntityFramework.IUnitOfWorkManager;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));
            For<EmployerAccountsDbContext>().Use(c => new EmployerAccountsDbContext(c.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString));
            For<IUnitOfWorkManager>().Use<UnitOfWorkManager<EmployerAccountsDbContext>>();
            For<EmployerFinanceDbContext>().Use(c => GetDbContext(c));
        }

        private EmployerFinanceDbContext GetDbContext(IContext context)
        {
            var unitOfWorkContext = context.GetInstance<IUnitOfWorkContext>();
            var clientSession = unitOfWorkContext.TryGet<IClientOutboxTransaction>();
            var serverSession = unitOfWorkContext.TryGet<SynchronizedStorageSession>();
            var sqlSession = clientSession?.GetSqlSession() ?? serverSession.GetSqlSession();

            return new EmployerFinanceDbContext(sqlSession.Connection, sqlSession.Transaction);
        }
    }
}