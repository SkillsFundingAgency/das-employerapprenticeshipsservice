using System;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Jobs.DependencyResolution
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

            For<EmployerFinanceDbContext>().Use(c => GetDbContext(c));
        }

        private EmployerFinanceDbContext GetDbContext(IContext context)
        {
            var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];

            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var connection = environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                ? new SqlConnection(GetConnectionString(context))
                : new SqlConnection
                {
                    ConnectionString = GetConnectionString(context),
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                };

            return new EmployerFinanceDbContext(connection);
        }

        private string GetConnectionString(IContext context)
        {
            return context.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString;
        }
    }
}