using SFA.DAS.EmployerFinance.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IHmrcDateService _hmrcDateService;

        public DasLevyService(ITransactionRepository transactionRepository, IHmrcDateService hmrcDateService)
        {
            _transactionRepository = transactionRepository;
            _hmrcDateService = hmrcDateService;
        }

        public Task<decimal> GetAccountBalance(long accountId)
        {
            return _transactionRepository.GetAccountBalance(accountId);
        }

        public Task<TransactionLine[]> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
        {
            return _transactionRepository.GetAccountTransactionsByDateRange(accountId, fromDate, toDate);
        }

        public async Task<T[]> GetAccountProviderPaymentsByDateRange<T>(long accountId, long ukprn, DateTime fromDate,
            DateTime toDate) where T : TransactionLine
        {
            var transactions = await _transactionRepository.GetAccountTransactionByProviderAndDateRange(
                accountId,
                ukprn,
                fromDate,
                toDate);

            var result = transactions.OfType<T>().ToArray();

            EnsureAllPayrollDatesAreSet(result);

            return result;
        }

		 public async Task<T[]> GetAccountCoursePaymentsByDateRange<T>(long accountId, long ukprn, string courseName,
		     int? courseLevel, int? pathwayCode, DateTime fromDate,
		     DateTime toDate) where T : TransactionLine
        {
            var transactions = await _transactionRepository.GetAccountCoursePaymentsByDateRange(
                accountId,
                ukprn,
                courseName,
                courseLevel,
                pathwayCode,
                fromDate,
                toDate);

            var result = transactions.OfType<T>().ToArray();

            EnsureAllPayrollDatesAreSet(result);

            return result;
        }

        public Task<string> GetProviderName(long ukprn, long accountId, string periodEnd)
        {
            return _transactionRepository.GetProviderName(ukprn, accountId, periodEnd);
        }

        public Task<int> GetPreviousAccountTransaction(long accountId, DateTime fromDate)
        {
            return _transactionRepository.GetPreviousTransactionsCount(accountId, fromDate);
        }

        public async Task<T[]> GetAccountLevyTransactionsByDateRange<T>(long accountId, DateTime fromDate,
            DateTime toDate) where T : TransactionLine
        {
            var transactions = await _transactionRepository.GetAccountLevyTransactionsByDateRange(
                accountId,
                fromDate,
                toDate);

            var result = transactions.OfType<T>().ToArray();

            EnsureAllPayrollDatesAreSet(result);

            return result;
        }

        private void EnsureAllPayrollDatesAreSet<T>(IEnumerable<T> transactions) where T : TransactionLine
        {
            foreach (var transaction in transactions)
            {
                if (!string.IsNullOrEmpty(transaction.PayrollYear) && transaction.PayrollMonth != 0)
                {
                    transaction.PayrollDate =
                        _hmrcDateService.GetDateFromPayrollYearMonth(transaction.PayrollYear, transaction.PayrollMonth);
                }
            }
        }
    }
}
