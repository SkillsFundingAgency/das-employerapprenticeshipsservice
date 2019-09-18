using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public interface IEmployerAccountsApiClient
    {
        Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken = new CancellationToken());
        Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken = new CancellationToken());
        Task Ping(CancellationToken cancellationToken = new CancellationToken());
        Task<bool> HasAgreementBeenSigned(HasAgreementBeenSignedRequest roleRequest, CancellationToken cancellationToken);
    }
}