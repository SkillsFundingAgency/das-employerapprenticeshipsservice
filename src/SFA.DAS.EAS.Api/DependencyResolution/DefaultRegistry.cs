using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.WebApi;
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

            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            For<EmployerAccountsDbContext>().Use(c => GetAcccountsDbContext(c));
            For<EmployerFinanceDbContext>().Use(c => GetFinanceDbContext(c));
        }

        private EmployerAccountsDbContext GetAcccountsDbContext(IContext context)
        {
            var db = new EmployerAccountsDbContext(context.GetInstance<DbConnection>(), null);
            return db;
        }

        private EmployerFinanceDbContext GetFinanceDbContext(IContext context)
        {
            var db = new EmployerFinanceDbContext(context.GetInstance<LevyDeclarationProviderConfiguration>().DatabaseConnectionString);
            db.Database.BeginTransaction();
            return db;
        }

        private string GetConnectionString(IContext context)
        {
            return context.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString;
        }
    }
}