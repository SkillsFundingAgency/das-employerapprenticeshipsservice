using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public interface IEmployerFinanceApiService
    {
        Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);

        Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);        

        Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<TotalPaymentsModel> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));

        Task<List<AccountBalance>> GetAccountBalances(List<string> accountIds);
   
        Task<TransferAllowance> GetTransferAllowance(string hashedAccountId);

        Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken));
    }
}