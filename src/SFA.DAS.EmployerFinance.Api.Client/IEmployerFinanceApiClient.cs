using SFA.DAS.EAS.Finance.Api.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public interface IEmployerFinanceApiClient
    {
        Task HealthCheck();

        Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);

        Task<ICollection<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);

        Task<FinanceStatisticsViewModel> GetFinanceStatistics();
    }
}