using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public class TokenServiceApiClient: ITokenServiceApiClient
{
    private readonly ITokenServiceApiClientConfiguration _configuration;
    private readonly ISecureTokenHttpClient _httpClient;


    public TokenServiceApiClient(ITokenServiceApiClientConfiguration configuration)
        : this(configuration, new SecureTokenHttpClient(configuration))
    {
    }

    internal TokenServiceApiClient(ITokenServiceApiClientConfiguration configuration, ISecureTokenHttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }
    public async Task<PrivilegedAccessToken> GetPrivilegedAccessTokenAsync()
    {
        Uri uri = new Uri(new Uri(_configuration.ApiBaseUrl), "api/PrivilegedAccess");
        return JsonConvert.DeserializeObject<PrivilegedAccessToken>(await _httpClient.GetAsync(uri.ToString()));
    }
}
