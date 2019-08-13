using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public interface IEmployerAccountsApiClient
    {
        Task HealthCheck();
        Task<Statistics> GetStatistics();
        Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken);
        Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken);
    }
}