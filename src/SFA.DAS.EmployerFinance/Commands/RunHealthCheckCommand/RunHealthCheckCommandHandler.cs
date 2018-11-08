using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models;

namespace SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommandHandler : AsyncRequestHandler<RunHealthCheckCommand>
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;
        private readonly IEmployerFinanceApiClient _employerFinanceApiClient;

        public RunHealthCheckCommandHandler(Lazy<EmployerFinanceDbContext> db, IEmployerFinanceApiClient employerFinanceApiClient)
        {
            _db = db;
            _employerFinanceApiClient = employerFinanceApiClient;
        }

        protected override async Task HandleCore(RunHealthCheckCommand message)
        {
            var healthCheck = new HealthCheck(message.UserRef.Value);

            await healthCheck.Run(_employerFinanceApiClient.HealthCheck);

            _db.Value.HealthChecks.Add(healthCheck);
        }
    }
}