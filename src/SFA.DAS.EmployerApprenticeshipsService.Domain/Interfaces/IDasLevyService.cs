using SFA.DAS.EAS.Domain.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<ICollection<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);

        Task<int> GetPreviousAccountTransaction(long AccountId, DateTime FromDate);

        Task<string> GetProviderName(int ukprn, long accountId, string periodEnd);
    }
}