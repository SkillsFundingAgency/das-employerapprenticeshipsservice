using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.LegalEntities;
using SFA.DAS.EntityFramework;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedAccountLegalEntities
{
    public class GetPagedAccountLegalEntitiesQueryHandler : IAsyncRequestHandler<GetPagedAccountLegalEntitiesQuery, GetPagedAccountLegalEntitiesResponse>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public GetPagedAccountLegalEntitiesQueryHandler(IConfigurationProvider configurationProvider, Lazy<EmployerAccountsDbContext> db)
        {
            _configurationProvider = configurationProvider;
            _db = db;
        }

        public async Task<GetPagedAccountLegalEntitiesResponse> Handle(GetPagedAccountLegalEntitiesQuery message)
        {
            var accountLegalEntitiesCountQuery = _db.Value.AccountLegalEntities.Where(ale => ale.Deleted == null).FutureCount();

            var accountLegalEntitiesQuery = _db.Value.AccountLegalEntities
                .Where(ale => ale.Deleted == null)
                .OrderBy(a => a.Id)
                .Skip(message.PageSize.Value * (message.PageNumber.Value - 1))
                .Take(message.PageSize.Value)
                .ProjectTo<AccountLegalEntityViewModel>(_configurationProvider)
                .Future();

            var accountLegalEntitiesCount = await accountLegalEntitiesCountQuery.ValueAsync();
            var accountLegalEntities = await accountLegalEntitiesQuery.ToListAsync();
            var totalPages = (accountLegalEntitiesCount + message.PageSize.Value - 1) / message.PageSize.Value;

            return new GetPagedAccountLegalEntitiesResponse
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
