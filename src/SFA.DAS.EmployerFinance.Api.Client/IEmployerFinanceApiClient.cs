using SFA.DAS.EmployerFinance.Api.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public interface IEmployerFinanceApiClient
    {
        Task HealthCheck();

        Task<List<LevyDeclaration>> GetLevyDeclarations(string hashedAccountId);

        Task<List<LevyDeclaration>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);

        Task<Transactions> GetTransactions(string accountId, int year, int month);

        Task<List<TransactionSummary>> GetTransactionSummary(string accountId);

        Task<TotalPaymentsModel> GetFinanceStatistics();
    }
}