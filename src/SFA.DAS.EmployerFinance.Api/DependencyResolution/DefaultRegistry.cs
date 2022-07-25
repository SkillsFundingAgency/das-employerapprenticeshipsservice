using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using NServiceBus.Persistence;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.Context;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string AzureResource = "https://database.windows.net/";

        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });


            var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
            environmentName = "AT";

            For<DbConnection>().Use($"Build DbConnection", c =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(GetEmployerFinanceConnectionString(c))
                    : new SqlConnection
                    {
                        ConnectionString = GetEmployerFinanceConnectionString(c),
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });




            For<EmployerFinanceDbContext>().Use(c => GetFinanceDbContext(c));
        }

        private EmployerFinanceDbContext GetFinanceDbContext(IContext context)
        {

            var db = new EmployerFinanceDbContext(context.GetInstance<DbConnection>());
            return db;

            /*var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
            environmentName = "AT";

            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var connection = environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                ? new SqlConnection(GetEmployerFinanceConnectionString(context))
                : new SqlConnection
                {
                    ConnectionString = GetEmployerFinanceConnectionString(context),
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                };

            //var unitOfWorkContext = context.GetInstance<IUnitOfWorkContext>();
            //var clientSession = unitOfWorkContext.Find<IClientOutboxTransaction>();
            //var serverSession = unitOfWorkContext.Find<SynchronizedStorageSession>();
            //var sqlSession = clientSession?.GetSqlSession() ?? serverSession.GetSqlSession();
            //return new EmployerFinanceDbContext(connection, sqlSession.Transaction); // TODO : check this

            return new EmployerFinanceDbContext(connection);*/
        }

        private string GetEmployerFinanceConnectionString(IContext context)
        {
            return context.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString;

            //return "Server=tcp:das-at-shared-sql.database.windows.net,1433;Initial Catalog=das-at-eas-fin-db;Persist Security Info=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        }
    }
}