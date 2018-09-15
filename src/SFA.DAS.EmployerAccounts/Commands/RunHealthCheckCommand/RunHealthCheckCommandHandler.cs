using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommandHandler : AsyncRequestHandler<RunHealthCheckCommand>
    {
        private readonly IEmployerAccountsApiClient _employerAccountsApiClient;
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public RunHealthCheckCommandHandler(IEmployerAccountsApiClient employerAccountsApiClient, Lazy<EmployerAccountsDbContext> db)
        {
            _employerAccountsApiClient = employerAccountsApiClient;
            _db = db;
        }

        protected override async Task HandleCore(RunHealthCheckCommand message)
        {
            var healthCheck = new HealthCheck(message.UserRef.Value);

            try
            {
                await healthCheck.SendRequest(_employerAccountsApiClient.HealthCheck);
            }
            catch
            {
            }

            try
            {
                healthCheck.PublishEvent();
            }
            catch
            {
            }

            _db.Value.HealthChecks.Add(healthCheck);
        }
    }
}