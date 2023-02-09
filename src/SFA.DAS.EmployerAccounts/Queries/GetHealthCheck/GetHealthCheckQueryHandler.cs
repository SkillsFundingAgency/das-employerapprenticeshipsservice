using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;

public class GetHealthCheckQueryHandler : IRequestHandler<GetHealthCheckQuery, GetHealthCheckQueryResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public GetHealthCheckQueryHandler(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public async Task<GetHealthCheckQueryResponse> Handle(GetHealthCheckQuery message, CancellationToken cancellationToken)
    {
        var healthCheck = await _db.Value.HealthChecks
            .OrderByDescending(h => h.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetHealthCheckQueryResponse
        {
            HealthCheck = new HealthCheckDto
            {
                Id = healthCheck.Id,
                PublishedEvent = healthCheck.PublishedEvent,
                ReceivedEvent = healthCheck.ReceivedEvent,
                ReceivedResponse = healthCheck.ReceivedResponse,
                SentRequest = healthCheck.SentRequest,
            }
        };
    }
}