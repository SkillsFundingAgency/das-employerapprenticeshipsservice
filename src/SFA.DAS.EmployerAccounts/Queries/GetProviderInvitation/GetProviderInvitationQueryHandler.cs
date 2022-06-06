using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation
{
    public class GetProviderInvitationQueryHandler : IAsyncRequestHandler<GetProviderInvitationQuery, GetProviderInvitationResponse>
    {
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly IHttpService _httpService;

        public GetProviderInvitationQueryHandler(
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

        public async Task<GetProviderInvitationResponse> Handle(GetProviderInvitationQuery message)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/invitations/{message.CorrelationId.ToString()}";
            var json = await _httpService.GetAsync(url, false);

            return new GetProviderInvitationResponse
            {
                Result = json == null ? null : JsonConvert.DeserializeObject<ProviderInvitation>(json)
            };
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