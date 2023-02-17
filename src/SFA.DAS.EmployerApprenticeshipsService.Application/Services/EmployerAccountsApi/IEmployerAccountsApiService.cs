using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi
{
    public interface IEmployerAccountsApiService
    {
        Task<Statistics> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));
        Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate, int pageSize, int pageNumber, CancellationToken cancellationToken = default(CancellationToken));
        Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken));
    }
}
