using System.Configuration;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NServiceBus.Persistence;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

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
            var connectionString = GetEmployerAccountsConnectionString(c);
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);
            if (useManagedIdentity)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result;
                return new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = accessToken,
                };
            }
            else
            {
                return new SqlConnection(connectionString);
            }
        });

        For<EmployerAccountsDbContext>().Use(c => GetEmployerAccountsDbContext(c));
    }

    private static EmployerAccountsDbContext GetEmployerAccountsDbContext(IContext context)
    {
        var unitOfWorkContext = context.GetInstance<IUnitOfWorkContext>();
        var clientSession = unitOfWorkContext.Find<IClientOutboxTransaction>();
        var serverSession = unitOfWorkContext.Find<SynchronizedStorageSession>();

        var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>();
        var connectionString = GetEmployerAccountsConnectionString(context);
        optionsBuilder.UseSqlServer(connectionString);
        return new EmployerAccountsDbContext(optionsBuilder.Options);
    }

    private static string GetEmployerAccountsConnectionString(IContext context)
    {
        return context.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString;
    }
}