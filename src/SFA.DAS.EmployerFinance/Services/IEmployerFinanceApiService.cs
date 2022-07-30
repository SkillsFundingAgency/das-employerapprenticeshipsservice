using SFA.DAS.EAS.Finance.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IEmployerFinanceApiService
    {

        Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);

        Task<ICollection<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);

        Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<FinanceStatisticsViewModel> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));
        Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate, int pageSize, int pageNumber, CancellationToken cancellationToken = default(CancellationToken));
        Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default(CancellationToken));
        Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken));


    }
}
