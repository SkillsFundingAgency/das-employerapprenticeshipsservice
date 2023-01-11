using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiRequests.HealthCheck;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiResponses.HealthCheck;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;

public class RunHealthCheckCommandHandler : AsyncRequestHandler<RunHealthCheckCommand>
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

    protected override async Task HandleCore(RunHealthCheckCommand message)
    {
        var healthCheck = new HealthCheck(message.UserRef ?? Guid.NewGuid());

        await healthCheck.Run(() => _accountsApiClient.Ping());
        await healthCheck.Run(() => _outerApiClient.Get<PingResponse>(new PingRequest()));

        _db.Value.HealthChecks.Add(healthCheck);
    }
}