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
                configuration.ProviderRelationsApi.ClientId,
                configuration.ProviderRelationsApi.ClientSecret,
                configuration.ProviderRelationsApi.IdentifierUri,
                configuration.ProviderRelationsApi.Tenant
            );
        }

        protected override async Task HandleCore(UnsubscribeProviderEmailQuery message)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}unsubscribe/{message.CorrelationId.ToString()}";
            await _httpService.GetAsync(url, false);
        }

        private string GetBaseUrl()
        {
            var baseUrl = _configuration.ProviderRelationsApi.BaseUrl.EndsWith("/")
                ? _configuration.ProviderRelationsApi.BaseUrl
                : _configuration.ProviderRelationsApi.BaseUrl + "/";

            return baseUrl;
        }
    }
}