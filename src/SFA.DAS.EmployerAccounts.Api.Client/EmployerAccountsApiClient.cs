using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        private readonly ISecureHttpClient _httpClient;
        private readonly IMediator _mediator;

        public EmployerAccountsApiClient(ISecureHttpClient httpClient, IMediator mediator)
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

        public Task Ping(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.WhenAll(_httpClient.GetAsync("ping", cancellationToken), _mediator.Send(new PingQuery(), cancellationToken));
        }
    }
}