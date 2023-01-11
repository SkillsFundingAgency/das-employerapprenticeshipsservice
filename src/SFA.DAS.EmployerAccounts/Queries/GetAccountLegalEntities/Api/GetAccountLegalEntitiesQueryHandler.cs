using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EntityFramework;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

public class GetAccountLegalEntitiesQueryHandler : IAsyncRequestHandler<GetAccountLegalEntitiesQuery, GetAccountLegalEntitiesResponse>
{
    private readonly IConfigurationProvider _configurationProvider;
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public GetAccountLegalEntitiesQueryHandler(IConfigurationProvider configurationProvider, Lazy<EmployerAccountsDbContext> db)
    {
        _configurationProvider = configurationProvider;
        _db = db;
    }

    public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesQuery message)
    {
        var accountLegalEntitiesCountQuery = _db.Value.AccountLegalEntities.Where(ale => ale.Deleted == null).FutureCount();

        var accountLegalEntitiesQuery = _db.Value.AccountLegalEntities
            .Where(ale => ale.Deleted == null)
            .OrderBy(a => a.Id)
            .Skip(message.PageSize.Value * (message.PageNumber.Value - 1))
            .Take(message.PageSize.Value)
            .ProjectTo<AccountLegalEntity>(_configurationProvider)
            .Future();

        var accountLegalEntitiesCount = await accountLegalEntitiesCountQuery.ValueAsync();
        var accountLegalEntities = await accountLegalEntitiesQuery.ToListAsync();
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