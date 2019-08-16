using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public interface IEmployerAccountsApiClient
    {
        Task HealthCheck();
        Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken);
        Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken);
    }
}