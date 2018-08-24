﻿using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransactionRepository
    {
        Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transaction);
		Task<List<TransactionLine>> GetAccountTransactionByProviderAndDateRange(long accountId, long ukprn, DateTime fromDate, DateTime toDate);
        Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate);
        Task<List<TransactionLine>> GetAccountTransactionsByDateRange(
            long accountId, DateTime fromDate, DateTime toDate);
 		Task<List<TransactionLine>> GetAccountCoursePaymentsByDateRange(long accountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate);
    }
}
