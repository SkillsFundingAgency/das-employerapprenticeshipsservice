using System.Net.Http;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi;

public class OuterApiClient : IOuterApiClient
{
    private readonly HttpClient _httpClient;
    
    public OuterApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);

        var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<TResponse>(json);
    }
}