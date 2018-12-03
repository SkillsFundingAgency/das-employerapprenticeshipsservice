using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EntityFramework;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.Queries.GetStatistics
{
    public class GetStatisticsQueryHandler : IAsyncRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _accountDb;
        private readonly EmployerFinanceDbContext _financeDb;

        public GetStatisticsQueryHandler(Lazy<EmployerAccountsDbContext> accountDb, EmployerFinanceDbContext financeDb)
        {
            _accountDb = accountDb;
            _financeDb = financeDb;
        }

        public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery message)
        {
            var accountsQuery = _accountDb.Value.Accounts.FutureCount();
            var legalEntitiesQuery = _accountDb.Value.LegalEntities.FutureCount();
            var payeSchemesQuery = _accountDb.Value.Payees.FutureCount();
            var agreementsQuery = _accountDb.Value.Agreements.Where(a => a.StatusId == EmployerAgreementStatus.Signed).FutureCount();
            var paymentsQuery = _financeDb.Payments.FutureCount();

            var statistics = new StatisticsViewModel
            {
                TotalAccounts = await accountsQuery.ValueAsync(),
                TotalLegalEntities = await legalEntitiesQuery.ValueAsync(),
                TotalPayeSchemes = await payeSchemesQuery.ValueAsync(),
                TotalAgreements = await agreementsQuery.ValueAsync(),
                TotalPayments = await paymentsQuery.ValueAsync()
            };

            return new GetStatisticsResponse
            {
                Statistics = statistics
            };
        }
    }
}