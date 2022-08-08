using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
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
            environmentName = "AT"; //TODO :  keep this to run locally for now then remove later

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
        }

        private string GetEmployerFinanceConnectionString(IContext context)
        {
            return context.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString;
        }
    }
}