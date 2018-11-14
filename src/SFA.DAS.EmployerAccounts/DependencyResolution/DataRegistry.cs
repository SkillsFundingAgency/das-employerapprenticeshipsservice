using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Documents;
using NServiceBus.Persistence;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.UnitOfWork;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<IDocumentClient>().Use(c => c.GetInstance<IDocumentClientFactory>().CreateDocumentClient()).Singleton();
            For<IDocumentClientFactory>().Use<DocumentClientFactory>();

            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString));
            For<EmployerAccountsDbContext>().Use(c => GetDbContext(c));
            For<EmployerFinanceDbContext>().Use(c => new EmployerFinanceDbContext(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));
        }

        private EmployerAccountsDbContext GetDbContext(IContext context)
        {
            var unitOfWorkContext = context.GetInstance<IUnitOfWorkContext>();
            var clientSession = unitOfWorkContext.TryGet<IClientOutboxTransaction>();
            var serverSession = unitOfWorkContext.TryGet<SynchronizedStorageSession>();
            var sqlSession = clientSession?.GetSqlSession() ?? serverSession.GetSqlSession();

            return new EmployerAccountsDbContext(sqlSession.Connection, sqlSession.Transaction);
        }
    }
}