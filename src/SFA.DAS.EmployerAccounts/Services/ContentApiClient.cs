using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Authentication.Extensions.Legacy;

namespace SFA.DAS.EmployerAccounts.Services;

public class ContentApiClient : ApiClientBase, IContentApiClient
{
    private readonly string _apiBaseUrl;        
    private readonly string _identifierUri;
    private readonly HttpClient _client;

    public ContentApiClient(HttpClient client, IContentClientApiConfiguration configuration) : base(client)
    {
        _apiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
            ? configuration.ApiBaseUrl
            : configuration.ApiBaseUrl + "/";

        _identifierUri = configuration.IdentifierUri;
        _client = client;
    }

    public async Task<string> Get(string type, string applicationId)
    {
        await AddAuthenticationHeader();

        var uri = $"{_apiBaseUrl}api/content?applicationId={applicationId}&type={type}";
        var content = await GetAsync(uri);

        return content;
    }

    private async Task AddAuthenticationHeader()
    {
        if (ConfigurationManager.AppSettings["EnvironmentName"].ToUpper() != "LOCAL")
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_identifierUri);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}