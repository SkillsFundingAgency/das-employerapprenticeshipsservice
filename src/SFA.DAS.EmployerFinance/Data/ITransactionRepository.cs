using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransactionRepository
    {
        Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transaction);
		Task<TransactionLine[]> GetAccountTransactionByProviderAndDateRange(long accountId, long ukprn, DateTime fromDate, DateTime toDate);
        Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate);
        Task<decimal> GetAccountBalance(long accountId);
        Task<TransactionLine[]> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);
 		Task<TransactionLine[]> GetAccountCoursePaymentsByDateRange(long accountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate);
        Task<TransactionLine[]> GetAccountLevyTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);
        Task<string> GetProviderName(long ukprn, long accountId, string periodEnd);
        Task<TransactionDownloadLine[]> GetAllTransactionDetailsForAccountByDate(long accountId, DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalSpendForLastYear(long accountId);

        Task<List<TransactionSummary>> GetAccountTransactionSummary(long accountId);
    }
}
