using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

public interface IEmployerFinanceApiService
{
    Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId, CancellationToken cancellationToken = default);

    Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth, CancellationToken cancellationToken = default);

    Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId, CancellationToken cancellationToken = default);

    Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month, CancellationToken cancellationToken = default);

    Task<TotalPaymentsModel> GetStatistics(CancellationToken cancellationToken = default);

    Task<List<AccountBalance>> GetAccountBalances(List<string> accountIds, CancellationToken cancellationToken = default);

    Task<TransferAllowance> GetTransferAllowance(string hashedAccountId, CancellationToken cancellationToken = default);

    Task<string> Redirect(string url, CancellationToken cancellationToken = default);
}