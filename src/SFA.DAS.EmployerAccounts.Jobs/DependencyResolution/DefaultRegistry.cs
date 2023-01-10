using System;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using StructureMap;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using NLog.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution
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

            For<ILoggerFactory>().Use(() => new LoggerFactory().AddApplicationInsights(ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"], null).AddNLog()).Singleton();
            For<ILogger>().Use(c => c.GetInstance<ILoggerFactory>().CreateLogger(c.ParentType));
            For<EmployerAccountsDbContext>().Use(c => GetDbContext(c));
            For<IRunOnceJobsService>().Use<RunOnceJobsService>();
        }

        private EmployerAccountsDbContext GetDbContext(IContext context)
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

            return new EmployerAccountsDbContext(connection);
        }

        private string GetConnectionString(IContext context)
        {
            return context.GetInstance<EmployerAccountsConfiguration>().SqlConnectionString;
        }
    }
}