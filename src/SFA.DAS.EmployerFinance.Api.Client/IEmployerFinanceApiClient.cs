using SFA.DAS.EAS.Finance.Api.Types;
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

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<List<TransactionSummary>> GetTransactionSummary(string accountId);

        Task<FinanceStatisticsViewModel> GetFinanceStatistics();
    }
}