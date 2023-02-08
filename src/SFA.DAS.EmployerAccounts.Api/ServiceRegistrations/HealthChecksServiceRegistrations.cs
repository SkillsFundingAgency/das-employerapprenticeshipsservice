using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Api.HealthChecks;
using SFA.DAS.EmployerAccounts.Api.Http;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations
{
    public static class HealthChecksServiceRegistrations
    {
        private const string AzureResource = "https://database.windows.net/";

        public static IServiceCollection AddDasHealthChecks(this IServiceCollection services, EmployerAccountsConfiguration configuration)
        {   
            void BeforeOpen(SqlConnection connection)
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
                bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);
                if (useManagedIdentity)
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    connection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result;
                }
            }

            services.AddHealthChecks()
                .AddCheck<NServiceBusHealthCheck>("Service Bus Health Check")
                .AddCheck<ReservationsApiHealthCheck>("Employer Accounts API Health Check")
                .AddSqlServer(configuration.DatabaseConnectionString, name: "Employer Accounts DB Health Check", beforeOpenConnectionConfigurer: BeforeOpen);

            return services;
        }

        public static IApplicationBuilder UseDasHealthChecks(this IApplicationBuilder app)
        {
            return app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = (c, r) => c.Response.WriteJsonAsync(new
                {
                    r.Status,
                    r.TotalDuration,
                    Results = r.Entries.ToDictionary(
                        e => e.Key,
                        e => new
                        {
                            e.Value.Status,
                            e.Value.Duration,
                            e.Value.Description,
                            e.Value.Data
                        })
                })
            });
        }
    }
}