using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasLevyService
    {
        Task<decimal> GetAccountBalance(long accountId);

        Task<TransactionLine[]> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate,
            DateTime toDate);

        Task<int> GetPreviousAccountTransaction(long accountId, DateTime fromDate);

        Task<T[]> GetAccountProviderPaymentsByDateRange<T>(long accountId, long ukprn, DateTime fromDate,
            DateTime toDate)
            where T : TransactionLine;


        Task<T[]> GetAccountCoursePaymentsByDateRange<T>(long accountId, long ukprn, string courseName,
            int? courseLevel, int? pathwayCode, DateTime fromDate,
            DateTime toDate)
            where T : TransactionLine;

        Task<T[]> GetAccountLevyTransactionsByDateRange<T>(long accountId, DateTime fromDate, DateTime toDate)
            where T : TransactionLine;

        Task<string> GetProviderName(long ukprn, long accountId, string periodEnd);
    }
}