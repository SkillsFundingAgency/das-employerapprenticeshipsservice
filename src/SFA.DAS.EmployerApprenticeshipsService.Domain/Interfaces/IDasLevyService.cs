using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<ICollection<TransactionLine>> GetTransactionsByAccountId(long accountId);

        Task<ICollection<AccountBalance>> GetAllAccountBalances();

        Task<ICollection<T>> GetTransactionsByDateRange<T>(
            long accountId, DateTime fromDate, DateTime toDate, string externalUserId)
            where T : TransactionLine;
    }
}