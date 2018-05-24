using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.Queries.GetStatistics
{
    public class GetStatisticsQueryHandler : IAsyncRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
    {
        private readonly EmployerAccountDbContext _accountDb;
        private readonly EmployerFinancialDbContext _financialDb;

        public GetStatisticsQueryHandler(EmployerAccountDbContext accountDb, EmployerFinancialDbContext financialDb)
        {
            _accountDb = accountDb;
            _financialDb = financialDb;
        }

        public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery message)
        {
            var accountsQuery = _accountDb.Accounts.FutureCount();
            var legalEntitiesQuery = _accountDb.LegalEntities.Where(l => l.Status == "active").FutureCount();
            var payeSchemesQuery = _accountDb.Payees.FutureCount();
            var agreementsQuery = _accountDb.Agreements.Where(a => a.StatusId == EmployerAgreementStatus.Signed).FutureCount();
            var paymentsQuery = _financialDb.Payments.Where(p => p.CollectionPeriodYear == DateTime.Now.Year).FutureCount();

            var statistics = new StatisticsViewModel
            {
                TotalAccounts = await accountsQuery.ValueAsync(),
                TotalActiveLegalEntities = await legalEntitiesQuery.ValueAsync(),
                TotalPayeSchemes = await payeSchemesQuery.ValueAsync(),
                TotalSignedAgreements = await agreementsQuery.ValueAsync(),
                TotalPaymentsThisYear = await paymentsQuery.ValueAsync()
            };

            return new GetStatisticsResponse
            {
                Statistics = statistics
            };
        }
    }
}
