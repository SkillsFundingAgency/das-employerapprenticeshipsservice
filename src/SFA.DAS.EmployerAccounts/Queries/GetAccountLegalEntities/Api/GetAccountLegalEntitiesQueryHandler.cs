using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Azure.Cosmos.Linq;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

public class GetAccountLegalEntitiesQueryHandler : IRequestHandler<GetAccountLegalEntitiesQuery, GetAccountLegalEntitiesResponse>
{
    private readonly IConfigurationProvider _configurationProvider;
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public GetAccountLegalEntitiesQueryHandler(IConfigurationProvider configurationProvider, Lazy<EmployerAccountsDbContext> db)
    {
        _configurationProvider = configurationProvider;
        _db = db;
    }

    public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesQuery message, CancellationToken cancellationToken)
    {
        var accountLegalEntitiesCount = await _db.Value.AccountLegalEntities.Where(ale => ale.Deleted == null).CountAsync(cancellationToken);

        var accountLegalEntities = await _db.Value.AccountLegalEntities
            .Where(ale => ale.Deleted == null)
            .OrderBy(a => a.Id)
            .Skip(message.PageSize.Value * (message.PageNumber.Value - 1))
            .Take(message.PageSize.Value)
            .ProjectTo<AccountLegalEntity>(_configurationProvider)
            .ToListAsync(cancellationToken);
        
        var totalPages = (accountLegalEntitiesCount + message.PageSize.Value - 1) / message.PageSize.Value;

        return new GetAccountLegalEntitiesResponse
        {
            AccountLegalEntities = new PagedApiResponse<AccountLegalEntity>
            {
                Data = accountLegalEntities,
                Page = message.PageNumber.Value,
                TotalPages = totalPages
            }
        };
    }
}