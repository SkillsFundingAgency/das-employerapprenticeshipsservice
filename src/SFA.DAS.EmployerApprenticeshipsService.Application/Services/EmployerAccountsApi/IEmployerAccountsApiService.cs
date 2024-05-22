using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

public interface IEmployerAccountsApiService
{
    Task<Statistics> GetStatistics(CancellationToken cancellationToken = default);

    Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate, int pageSize, int pageNumber, CancellationToken cancellationToken = default);

    Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default);
    Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default);
    Task ChangeRole(string hashedAccountId, string email, int role, string supportUserEmail, CancellationToken cancellationToken = default);
    Task ResendInvitation(string hashedAccountId, string email, string supportUserEmail, CancellationToken cancellationToken = default);
}