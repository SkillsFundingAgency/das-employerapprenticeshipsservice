using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        private readonly IEmployerAccountsApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;
        private readonly IReadStoreMediator _mediator;

        public EmployerAccountsApiClient(IEmployerAccountsApiClientConfiguration configuration, IReadStoreMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
            _httpClient = new SecureHttpClient(configuration);
        }

        public Task HealthCheck()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/healthcheck";

            return _httpClient.GetAsync(url);
        }

        public Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            return _mediator.Send(new IsUserInRoleQuery(
                roleRequest.UserRef,
                roleRequest.AccountId,
                new HashSet<UserRole>(roleRequest.Roles)
            ), cancellationToken);
        }

        public Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            return _mediator.Send(new IsUserInAnyRoleQuery(
                roleRequest.UserRef,
                roleRequest.AccountId), cancellationToken);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.Trim('/');
        }
    }
}