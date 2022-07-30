using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SFA.DAS.EAS.Finance.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public interface IEmployerFinanceApiService
    {

        Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);

        Task<ICollection<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth);        

        Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);

        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);

        Task<Statistics> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));
        Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate, int pageSize, int pageNumber, CancellationToken cancellationToken = default(CancellationToken));
        Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default(CancellationToken));
        Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken));


    }
}
