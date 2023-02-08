using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.EmployerAccounts.Extensions;

public static class DatabaseExtensions
{
    private const string AzureResource = "https://database.windows.net/";

    public static DbConnection GetSqlConnection(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);
        
        if (!useManagedIdentity)
        {
            return new SqlConnection(connectionString);
        }

        var azureServiceTokenProvider = new AzureServiceTokenProvider();

        return new SqlConnection
        {
            ConnectionString = connectionString,
            AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
        };
    }
}