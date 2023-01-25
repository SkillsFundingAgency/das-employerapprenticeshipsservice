using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Api.HealthChecks;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations
{
    public static class HealthChecksServiceRegistrations
    {
        private const string AzureResource = "https://database.windows.net/";

        public static IServiceCollection AddDasHealthChecks(this IServiceCollection services, EmployerAccountsConfiguration configuration)
        {
            var environmentName = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);
            
            void BeforeOpen(SqlConnection connection)
            {
                if (!environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
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