using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<List<TransactionLine>> GetTransactionsByAccountId(long accountId);

        Task<List<AccountBalance>> GetAllAccountBalances();

        Task<List<TransactionLineDetail>> GetTransactionDetailByDateRange(
            long accountId, DateTime fromDate, DateTime toDate, string externalUserId);
    }
}