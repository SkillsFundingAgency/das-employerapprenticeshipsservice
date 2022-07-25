using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using NServiceBus.Persistence;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.Context;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class DataRegistry : Registry
    {
        private const string AzureResource = "https://database.windows.net/";

        public DataRegistry()
        {
            var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
            environmentName = "AT";

            For<DbConnection>().Use($"Build DbConnection", c =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(GetConnectionString(c))
                    : new SqlConnection
                    {
                        ConnectionString = GetConnectionString(c),
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });

            For<EmployerFinanceDbContext>().Use(c => GetDbContext(c));
        }

        private EmployerFinanceDbContext GetDbContext(IContext context)
        {
            var unitOfWorkContext = context.GetInstance<IUnitOfWorkContext>();
            var clientSession = unitOfWorkContext.Find<IClientOutboxTransaction>();
            var serverSession = unitOfWorkContext.Find<SynchronizedStorageSession>();
            var sqlSession = clientSession?.GetSqlSession() ?? serverSession.GetSqlSession();

            return new EmployerFinanceDbContext(sqlSession.Connection, sqlSession.Transaction);

            //return new EmployerFinanceDbContext(sqlSession.Connection);
        }

        private string GetConnectionString(IContext context)
        {
            return context.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString;

            //return "Server=tcp:das-at-shared-sql.database.windows.net,1433;Initial Catalog=das-at-eas-fin-db;Persist Security Info=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        
        }
    }
}