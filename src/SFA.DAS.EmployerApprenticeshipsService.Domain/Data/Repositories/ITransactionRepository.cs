﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);
        
        Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate);

        Task<List<TransactionSummary>> GetAccountTransactionSummary(long accountId);

        Task<string> GetProviderName(int ukprn, long accountId, string periodEnd);
    }
}
