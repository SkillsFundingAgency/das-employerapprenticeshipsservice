using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi
{
    public interface IEmployerAccountsApiService
    {
        Task<Statistics> GetStatistics(CancellationToken cancellationToken = default(CancellationToken));
    }
}
