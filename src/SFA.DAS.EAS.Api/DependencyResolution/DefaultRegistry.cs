using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using StructureMap;

namespace SFA.DAS.EAS.Account.Api.DependencyResolution
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
            environmentName = "TEST";

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

           
            For<EmployerAccountsDbContext>().Use(c => GetAcccountsDbContext(c));
        }

        private EmployerAccountsDbContext GetAcccountsDbContext(IContext context)
        {
            var db = new EmployerAccountsDbContext(context.GetInstance<DbConnection>());
            return db;
        }

        private string GetEmployerAccountsConnectionString(IContext context)
        {
            return context.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString;
        }
    }
}