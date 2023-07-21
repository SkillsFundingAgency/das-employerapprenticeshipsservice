using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

public class GetAccountLegalEntitiesQueryHandler : IRequestHandler<GetAccountLegalEntitiesQuery, GetAccountLegalEntitiesResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IMapper _mapper;

    public GetAccountLegalEntitiesQueryHandler(Lazy<EmployerAccountsDbContext> db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesQuery message, CancellationToken cancellationToken)
    {
        var accountLegalEntitiesCount = await _db.Value.AccountLegalEntities.Where(ale => ale.Deleted == null).CountAsync(cancellationToken);

        var accountLegalEntities = _db.Value.AccountLegalEntities
            .Where(ale => ale.Deleted == null)
            .OrderBy(a => a.Id)
            .Skip(message.PageSize.Value * (message.PageNumber.Value - 1))
            .Take(message.PageSize.Value);

        var entities = await _mapper
            .ProjectTo<AccountLegalEntity>(accountLegalEntities)
            .ToListAsync(cancellationToken);
        
        var totalPages = (accountLegalEntitiesCount + message.PageSize.Value - 1) / message.PageSize.Value;

        return new GetAccountLegalEntitiesResponse
        {
            AccountLegalEntities = new PagedApiResponse<AccountLegalEntity>
            {
                Data = entities,
                Page = message.PageNumber.Value,
                TotalPages = totalPages
            }
        };
    }
}