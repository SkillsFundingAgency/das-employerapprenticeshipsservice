using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransactionRepository
    {
        Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transaction);
        Task<List<TransactionLine>> GetAccountTransactionByProviderAndDateRange(long accountId, long ukprn, DateTime fromDate, DateTime toDate);
    }
}
