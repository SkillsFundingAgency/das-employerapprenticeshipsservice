using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail
{
    public class UnsubscribeProviderEmailQueryHandler : AsyncRequestHandler<UnsubscribeProviderEmailQuery>
    {
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly IHttpService _httpService;

        public UnsubscribeProviderEmailQueryHandler(
            IHttpServiceFactory httpServiceFactory,
            EmployerAccountsConfiguration configuration)
        {
            _configuration = configuration;         
            _httpService = httpServiceFactory.Create(
                configuration.ProviderRegistrationsApi.IdentifierUri,
                configuration.ProviderRegistrationsApi.ClientId,
                configuration.ProviderRegistrationsApi.ClientSecret,                
                configuration.ProviderRegistrationsApi.Tenant
            );
        }

        protected override async Task HandleCore(UnsubscribeProviderEmailQuery message)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/unsubscribe/{message.CorrelationId.ToString()}";
            await _httpService.GetAsync(url, false);
        }

        private string GetBaseUrl()
        {
            var baseUrl = _configuration.ProviderRegistrationsApi.BaseUrl.EndsWith("/")
                ? _configuration.ProviderRegistrationsApi.BaseUrl
                : _configuration.ProviderRegistrationsApi.BaseUrl + "/";

            return baseUrl;
        }
    }
}