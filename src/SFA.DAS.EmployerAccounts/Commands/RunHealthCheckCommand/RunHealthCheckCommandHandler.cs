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
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IEmployerAccountsApiClient _employerAccountsApiClient;

        public RunHealthCheckCommandHandler(Lazy<EmployerAccountsDbContext> db, IEmployerAccountsApiClient employerAccountsApiClient)
        {
            _db = db;
            _employerAccountsApiClient = employerAccountsApiClient;
        }

        protected override async Task HandleCore(RunHealthCheckCommand message)
        {
            var healthCheck = new HealthCheck(message.UserRef.Value);

            await healthCheck.Run(_employerAccountsApiClient.HealthCheck);

            _db.Value.HealthChecks.Add(healthCheck);
        }
    }
}