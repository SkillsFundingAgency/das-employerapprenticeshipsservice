using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Finance.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public interface IEmployerFinanceApiService
    {
        Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);

        Task<ICollection<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);        

        Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<FinanceStatisticsViewModel> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));

        Task<GetAccountBalancesResponse> GetAccountBalances(List<long> accountIds);
    }
}
