using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models;

namespace SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommandHandler : AsyncRequestHandler<RunHealthCheckCommand>
    {
        private readonly IEmployerFinanceApiClient _employerFinanceApiClient;
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public RunHealthCheckCommandHandler(IEmployerFinanceApiClient employerFinanceApiClient, Lazy<EmployerFinanceDbContext> db)
        {
            _employerFinanceApiClient = employerFinanceApiClient;
            _db = db;
        }

        protected override async Task HandleCore(RunHealthCheckCommand message)
        {
            var healthCheck = new HealthCheck(message.UserRef.Value);

            try
            {
                await healthCheck.SendRequest(_employerFinanceApiClient.HealthCheck);
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