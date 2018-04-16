using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Domain.Models.Transfers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);

        Task<List<TransactionLine>> GetAccountLevyTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);

        Task<List<TransactionLine>> GetAccountTransactionByProviderAndDateRange(long accountId, long ukprn, DateTime fromDate, DateTime toDate);

        Task<List<TransactionLine>> GetAccountCoursePaymentsByDateRange(long accountId, long ukprn, string courseName, int courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate);

        Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate);

        Task<List<TransactionSummary>> GetAccountTransactionSummary(long accountId);

        Task<List<TransactionDownloadLine>> GetAllTransactionDetailsForAccountByDate(long accountId, DateTime fromDate, DateTime toDate);

        Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transaction);

    }
}
