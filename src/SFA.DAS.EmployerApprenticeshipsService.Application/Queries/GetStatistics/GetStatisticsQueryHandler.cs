using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Infrastructure.Data;
using Z.EntityFramework.Plus;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.Queries.GetStatistics
{
    public class GetStatisticsQueryHandler : IAsyncRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
    {
        private readonly EmployerFinancialDbContext _financialDb;
        private readonly EmployerAccountDbContext _accountDb;

        public GetStatisticsQueryHandler(EmployerFinancialDbContext financialDb, EmployerAccountDbContext accountDb)
        {
            _financialDb = financialDb;
            _accountDb = accountDb;
        }

        public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery message)
        {
            var accountContextQuery = _accountDb.Accounts.DeferredCount().FutureValue();

            var legalEntityContextQuery = _accountDb.LegalEntities.DeferredCount(x => x.Status == "active").FutureValue();

            var payeContextQuery = _accountDb.Payees.DeferredCount().FutureValue();

            var employerAgreementContextQuery = _accountDb.Agreements.DeferredCount(x=>x.StatusId == EmployerAgreementStatus.Signed).FutureValue();


            var statisticsView = new StatisticsViewModel();

            if (accountContextQuery != null)
            {
                statisticsView.TotalAccounts = accountContextQuery.Value;
            }

            if (legalEntityContextQuery != null)
            {
                statisticsView.TotalActiveLegalEntities = legalEntityContextQuery.Value;
            }

            if (employerAgreementContextQuery != null)
            {
                statisticsView.TotalSignedAgreements = employerAgreementContextQuery.Value;
            }

            if (payeContextQuery != null)
            {
                statisticsView.TotalPAYESchemes = payeContextQuery.Value;
            }


            var paymentsContextQuery = _financialDb.Payments.DeferredCount(x => x.CollectionPeriodYear == DateTime.Now.Year).FutureValue();

            if (paymentsContextQuery != null)
            {
                statisticsView.TotalPaymentsThisYear = paymentsContextQuery.Value;
            }

            return new GetStatisticsResponse
            {
                Statistics = statisticsView
            };
        }
    }
}
