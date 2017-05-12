using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<ICollection<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);

        Task<ICollection<AccountBalance>> GetAllAccountBalances();

        Task<ICollection<T>> GetAccountProviderTransactionsByDateRange<T>(
            long accountId, DateTime fromDate, DateTime toDate, string externalUserId)
            where T : TransactionLine;

        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef);

        Task<int> GetPreviousAccountTransaction(long AccountId, DateTime FromDate, string externalUserId);
    }
}