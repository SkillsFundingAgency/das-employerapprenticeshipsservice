using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommandHandler : AsyncRequestHandler<RunHealthCheckCommand>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IAccountApiClient _accountsApiClient;        

        public RunHealthCheckCommandHandler(Lazy<EmployerAccountsDbContext> db, IAccountApiClient accountsApiClient)
        {
            _db = db;
            _accountsApiClient = accountsApiClient;
        }

        protected override async Task HandleCore(RunHealthCheckCommand message)
        {
            var healthCheck = new HealthCheck(message.UserRef ?? Guid.NewGuid());

            await healthCheck.Run(() => _accountsApiClient.Ping());

            _db.Value.HealthChecks.Add(healthCheck);
        }
    }
}