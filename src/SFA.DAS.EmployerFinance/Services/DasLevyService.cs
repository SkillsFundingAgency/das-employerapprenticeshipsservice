using MediatR;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetAccountTransactions;
using SFA.DAS.EmployerFinance.Queries.GetPreviousTransactionsCount;
using SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountProviderPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountCoursePayments;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;

        public DasLevyService(IMediator mediator)
        {
            _mediator = mediator;
        }
		
		public async Task<ICollection<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await _mediator.SendAsync(new GetAccountTransactionsRequest
            {
                AccountId = accountId,
                UkPrn = ukprn,
                FromDate = fromDate,
                ToDate = toDate
            });

            return result.TransactionLines;
        }

        public async Task<ICollection<T>> GetAccountProviderPaymentsByDateRange<T>(
            long accountId, long ukprn, DateTime fromDate, DateTime toDate) where T : TransactionLine
        {
            var result = await _mediator.SendAsync(new GetAccountProviderPaymentsByDateRangeQuery
            {
                AccountId = accountId,
                UkPrn = ukprn,
                FromDate = fromDate,
                ToDate = toDate
            });

            return result?.Transactions?.OfType<T>().ToList() ?? new List<T>();
        }
		 public async Task<ICollection<T>> GetAccountCoursePaymentsByDateRange<T>(
            long accountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode, DateTime fromDate,
            DateTime toDate) where T : TransactionLine
        {
            var result = await _mediator.SendAsync(new GetAccountCoursePaymentsQuery
            {
                AccountId = accountId,
                UkPrn = ukprn,
                CourseName = courseName,
                CourseLevel = courseLevel,
                PathwayCode = pathwayCode,
                FromDate = fromDate,
                ToDate = toDate
            });

            return result?.Transactions?.OfType<T>().ToList() ?? new List<T>();
        }

        public async Task<int> GetPreviousAccountTransaction(long accountId, DateTime fromDate)
        {
            var result = await _mediator.SendAsync(new GetPreviousTransactionsCountRequest
            {
                AccountId = accountId,
                FromDate = fromDate
            });

            return result.Count;
        }

    }
}
