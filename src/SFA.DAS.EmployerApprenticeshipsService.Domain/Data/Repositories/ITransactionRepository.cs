using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);

        Task<List<TransactionLine>> GetAccountLevyTransactionsByDateRange(long accountId, DateTime fromDate,
            DateTime toDate);

        Task<List<TransactionLine>> GetAccountTransactionByProviderAndDateRange(long accountId, long ukprn, DateTime fromDate, DateTime toDate);

        Task<List<TransactionLine>> GetCourseTransactionByDateRange(string courseName, long accountId, DateTime fromDate, DateTime toDate);

        Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate);
    }
}
