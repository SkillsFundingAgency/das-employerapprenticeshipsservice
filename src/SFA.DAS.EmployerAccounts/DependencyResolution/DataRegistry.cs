using System;
using System.Configuration;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Services.AppAuthentication;
using NServiceBus.Persistence;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.Context;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class DataRegistry : Registry
    {
        private const string AzureResource = "https://database.windows.net/";

        public DataRegistry()
        {
            var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];

            For<IDocumentClient>().Use(c => c.GetInstance<IDocumentClientFactory>().CreateDocumentClient()).Singleton();
            For<IDocumentClientFactory>().Use<DocumentClientFactory>();

            For<DbConnection>().Use($"Build DbConnection", c =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(GetEmployerAccountsConnectionString(c))
                    : new SqlConnection
                    {
                        ConnectionString = GetEmployerAccountsConnectionString(c),
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });

            For<EmployerAccountsDbContext>().Use(c => GetEmployerAccountsDbContext(c));
        }

        private EmployerAccountsDbContext GetEmployerAccountsDbContext(IContext context)
        {
            var unitOfWorkContext = context.GetInstance<IUnitOfWorkContext>();
            var clientSession = unitOfWorkContext.Find<IClientOutboxTransaction>();
            var serverSession = unitOfWorkContext.Find<SynchronizedStorageSession>();
            var sqlSession = clientSession?.GetSqlSession() ?? serverSession.GetSqlSession();

            return new EmployerAccountsDbContext(sqlSession.Connection, sqlSession.Transaction);
        }

        private string GetEmployerAccountsConnectionString(IContext context)
        {
            return context.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString;
        }
    }
}