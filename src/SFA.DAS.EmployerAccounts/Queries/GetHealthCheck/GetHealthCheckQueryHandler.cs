using System.Data.Entity;
using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;

public class GetHealthCheckQueryHandler : IRequestHandler<GetHealthCheckQuery, GetHealthCheckQueryResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IConfigurationProvider _configurationProvider;

    public GetHealthCheckQueryHandler(Lazy<EmployerAccountsDbContext> db, IConfigurationProvider configurationProvider)
    {
        _db = db;
        _configurationProvider = configurationProvider;
    }

    public async Task<GetHealthCheckQueryResponse> Handle(GetHealthCheckQuery message, CancellationToken cancellationToken)
    {
        var healthCheck = await _db.Value.HealthChecks
            .OrderByDescending(h => h.Id)
            .ProjectTo<HealthCheckDto>(_configurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetHealthCheckQueryResponse
        {
            HealthCheck = healthCheck
        };
    }
}