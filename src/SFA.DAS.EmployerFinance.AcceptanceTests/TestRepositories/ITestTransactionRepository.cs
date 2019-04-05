using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories
{
    public interface ITestTransactionRepository
    {
        Task CreateTransactionLines(IEnumerable<TransactionLineEntity> transactionLines);

        Task RemovePayeRef(string empRef);

        Task SetTransactionLineDateCreatedToTransactionDate(IEnumerable<long> submissionIds);

        Task SetTransactionLineDateCreatedToTransactionDate(IDictionary<long, DateTime?> submissionIds);

        Task<int> GetMaxAccountId();
    }
}