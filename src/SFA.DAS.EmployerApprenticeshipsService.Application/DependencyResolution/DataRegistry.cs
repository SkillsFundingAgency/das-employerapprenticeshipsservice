using System.Data.Common;
using System.Data.SqlClient;
using NServiceBus.Persistence;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EntityFramework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.UnitOfWork;
using StructureMap;
using IUnitOfWorkManager = SFA.DAS.EntityFramework.IUnitOfWorkManager;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            For<EmployerFinanceDbContext>().Use(c => new EmployerFinanceDbContext(c.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString));
            For<IUnitOfWorkManager>().Use<UnitOfWorkManager<EmployerFinanceDbContext>>();
            For<EmployerAccountsDbContext>().Use(c => GetDbContext(c));
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