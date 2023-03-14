using System.Threading;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.HealthCheck;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.HealthCheck;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;

public class RunHealthCheckCommandHandler : IRequestHandler<RunHealthCheckCommand>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IAccountApiClient _accountsApiClient;
    private readonly IOuterApiClient _outerApiClient;

    public RunHealthCheckCommandHandler(Lazy<EmployerAccountsDbContext> db, IAccountApiClient accountsApiClient, IOuterApiClient outerApiClient)
    {
        _db = db;
        _accountsApiClient = accountsApiClient;
        _outerApiClient = outerApiClient;
    }

    public async Task<Unit> Handle(RunHealthCheckCommand request, CancellationToken cancellationToken)
    {
        var healthCheck = new HealthCheck(request.UserRef ?? Guid.NewGuid());

        await healthCheck.Run(() => _accountsApiClient.Ping());
        await healthCheck.Run(() => _outerApiClient.Get<PingResponse>(new PingRequest()));

        _db.Value.HealthChecks.Add(healthCheck);

        return Unit.Value;
    }
}