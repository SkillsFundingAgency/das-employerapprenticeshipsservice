using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    //todo: not a great client/http combo
    public class EmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        private readonly ISecureHttpClient _httpClient;
        private readonly IReadStoreMediator _mediator;

        public EmployerAccountsApiClient(ISecureHttpClient httpClient, IReadStoreMediator mediator)
        {
            _httpClient = httpClient;
            _mediator = mediator;
        }

        public Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken = new CancellationToken())
        {
            return _mediator.Send(new IsUserInRoleQuery(
                roleRequest.UserRef,
                roleRequest.AccountId,
                roleRequest.Roles
            ), cancellationToken);
        }

        public Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken = new CancellationToken())
        {
            return _mediator.Send(new IsUserInAnyRoleQuery(
                roleRequest.UserRef,
                roleRequest.AccountId), cancellationToken);
        }

        public Task<bool> HasAgreementBeenSigned(HasAgreementBeenSignedRequest hasAgreementBeenSignedRequest, CancellationToken cancellationToken)
        {
            return _mediator.Send(new HasAgreementBeenSignedQuery(
                hasAgreementBeenSignedRequest.AccountId,
                hasAgreementBeenSignedRequest.AgreementVersion,
                hasAgreementBeenSignedRequest.AgreementType), cancellationToken);
        }

	public Task Ping(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.WhenAll(_httpClient.GetAsync("ping", cancellationToken), _mediator.Send(new PingQuery(), cancellationToken));
        }
    }
}