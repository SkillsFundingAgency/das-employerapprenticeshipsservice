using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authentication.Extensions.Legacy;

namespace SFA.DAS.EmployerAccounts.Services;

public class ProviderRegistrationApiClient : ApiClientBase, IProviderRegistrationApiClient
{
    private readonly string _apiBaseUrl;
    private readonly string _identifierUri;
    private readonly HttpClient _client;
    private readonly ILogger<ProviderRegistrationApiClient> _logger;

    public ProviderRegistrationApiClient(HttpClient client, IProviderRegistrationClientApiConfiguration configuration, ILogger<ProviderRegistrationApiClient> logger) : base(client)
    {
        _apiBaseUrl = configuration.BaseUrl.EndsWith("/")
            ? configuration.BaseUrl
            : configuration.BaseUrl + "/";

        _identifierUri = configuration.IdentifierUri;
        _client = client;
        _logger = logger;
    }

    public async Task Unsubscribe(string CorrelationId)
    {
        await AddAuthenticationHeader();           

        var url = $"{_apiBaseUrl}api/unsubscribe/{CorrelationId}";

        _logger.LogInformation("Getting Unsubscribe {Url}", url);

        await _client.GetAsync(url);
    }

    public async Task<string> GetInvitations(string CorrelationId)
    {
        await AddAuthenticationHeader();
            
        var url = $"{_apiBaseUrl}api/invitations/{CorrelationId}";
        _logger.LogInformation("Getting Invitations {Url}", url);
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    private async Task AddAuthenticationHeader()
    {
        if (!string.IsNullOrEmpty(_identifierUri))
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_identifierUri);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}