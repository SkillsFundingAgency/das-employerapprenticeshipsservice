using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Contracts.OuterApi;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Infrastructure.OuterApi;

public class OuterApiClient : IOuterApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OuterApiConfiguration _configuration;

    public OuterApiClient(HttpClient httpClient, OuterApiConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
    {
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        AddAuthenticationHeader(httpRequestMessage);
        
        using var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<TResponse>(json);
    }
    
    private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _configuration.Key);
        httpRequestMessage.Headers.Add("X-Version", "1");
    }
}