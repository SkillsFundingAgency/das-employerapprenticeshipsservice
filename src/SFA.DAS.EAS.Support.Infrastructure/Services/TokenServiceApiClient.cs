using Newtonsoft.Json;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Infrastructure.Settings;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public class TokenServiceApiClient : ITokenServiceApiClient
{
    private readonly ITokenServiceApiClientConfiguration _configuration;
    private readonly ISecureTokenHttpClient _httpClient;


    public TokenServiceApiClient(ITokenServiceApiClientConfiguration configuration)
        : this(configuration, new SecureTokenHttpClient(configuration)) { }

    internal TokenServiceApiClient(ITokenServiceApiClientConfiguration configuration, ISecureTokenHttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<PrivilegedAccessToken> GetPrivilegedAccessTokenAsync()
    {
        Uri uri = new(new Uri(_configuration.ApiBaseUrl), "api/PrivilegedAccess");

        return JsonConvert.DeserializeObject<PrivilegedAccessToken>(await _httpClient.GetAsync(uri.ToString()));
    }
}
