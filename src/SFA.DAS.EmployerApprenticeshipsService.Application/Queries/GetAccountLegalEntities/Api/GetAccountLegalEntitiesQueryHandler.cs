using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api
{
    public class GetAccountLegalEntitiesQueryHandler : IAsyncRequestHandler<GetAccountLegalEntitiesQuery, GetAccountLegalEntitiesResponse>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Lazy<EmployerAccountDbContext> _db;

        public GetAccountLegalEntitiesQueryHandler(IConfigurationProvider configurationProvider, Lazy<EmployerAccountDbContext> db)
        {
            _configurationProvider = configurationProvider;
            _db = db;
        }

        public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesQuery message)
        {
            var accountLegalEntitiesCountQuery = _db.Value.AccountLegalEntities.Where(ale => ale.Deleted == null).FutureCount();

            var accountLegalEntitiesQuery = _db.Value.AccountLegalEntities
                .Where(ale => ale.Deleted == null)
                .Skip(message.PageSize.Value * (message.PageNumber.Value - 1))
                .Take(message.PageSize.Value)
                .OrderBy(a => a.Id)
                .ProjectTo<AccountLegalEntityViewModel>(_configurationProvider)
                .Future();

            var accountLegalEntitiesCount = await accountLegalEntitiesCountQuery.ValueAsync();
            var accountLegalEntities = await accountLegalEntitiesQuery.ToListAsync();
            var totalPages = (accountLegalEntitiesCount + message.PageSize.Value - 1) / message.PageSize.Value;

            return new GetAccountLegalEntitiesResponse
            {
                AccountLegalEntities = new PagedApiResponseViewModel<AccountLegalEntityViewModel>
                {
                    Data = accountLegalEntities,
                    Page = message.PageNumber.Value,
                    TotalPages = totalPages
                }
            };
        }
    }
}
