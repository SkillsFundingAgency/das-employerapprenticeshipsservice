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
            var sqlSession = clientSession?.GetSqlSession() ?? serverSession?.GetSqlSession();

            if(sqlSession != null)
            {
                return new EmployerFinanceDbContext(sqlSession.Connection, sqlSession.Transaction);
            }
            else
            {
                // during the owin setup the NServiceBus storage session is not available so
                // the context cannot be constructed using the unit of work, this would mean
                // that a message cannot be published atomically with a database update
                var dbConnection = context.GetInstance<DbConnection>();
                return new EmployerFinanceDbContext(dbConnection);
            }
        }

        private string GetConnectionString(IContext context)
        {
            return context.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString;
        }
    }
}