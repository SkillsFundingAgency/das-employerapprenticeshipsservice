using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.EmployerAccounts.Api.HealthChecks
{
    public class ReservationsApiHealthCheck : IHealthCheck
    {
        private readonly IAccountApiClient _accountsApiClient;

        public ReservationsApiHealthCheck(IAccountApiClient accountsApiClient)
        {
            _accountsApiClient = accountsApiClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _accountsApiClient.Ping();
                
                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Degraded(exception.Message);
            }
        }
    }
}