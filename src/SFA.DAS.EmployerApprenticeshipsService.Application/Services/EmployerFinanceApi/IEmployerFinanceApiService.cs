using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
//using SFA.DAS.EAS.Finance.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public interface IEmployerFinanceApiService
    {
        Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);

        Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);        

        Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<FinanceStatisticsViewModel> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));
        
        Task<GetAccountBalancesResponse> GetAccountBalances(BulkAccountsRequest accountIds); //TODO : change to hashedAccountIds

        Task<GetTransferAllowanceResponse> GetTransferAllowance(long accountId); //TODO : change to hashedAccountId
    }
}
