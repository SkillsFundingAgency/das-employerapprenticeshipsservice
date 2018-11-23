using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Queries;
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

        public async Task<bool> HasRole(HasRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new HasRoleQuery(
                roleRequest.UserRef,
                roleRequest.EmployerAccountId,
                roleRequest.Roles.Select(r => (UserRole) r).ToArray()
            ), cancellationToken);

            return result.HasRole;
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.Trim('/');
        }
    }

    public class HasRoleRequest
    {
        public Guid UserRef { get; set; }
        public long EmployerAccountId { get; set; }
        public Role[] Roles { get; set; }
    }

    public enum Role
    {
        Owner = 1,
        Transactor = 2,
        Viewer = 3
    }
}