using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Infrastructure.Settings;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public class LevyTokenHttpClientFactory: ILevyTokenHttpClientFactory
{
    private readonly IHmrcApiClientConfiguration _hmrcApiBaseUrlConfig;
    private readonly ITokenServiceApiClient _tokenServiceApiClient;

    public LevyTokenHttpClientFactory(ITokenServiceApiClient tokenServiceApiClient, IHmrcApiClientConfiguration hmrcApiBaseUrlConfig)
    {
        _hmrcApiBaseUrlConfig = hmrcApiBaseUrlConfig;
        _tokenServiceApiClient = tokenServiceApiClient;
    }

    public async  Task<IApprenticeshipLevyApiClient> GetLevyHttpClient()
    {
        var tokenResult = await _tokenServiceApiClient.GetPrivilegedAccessTokenAsync();
        var httpclient = ApprenticeshipLevyApiClient.CreateHttpClient(tokenResult.AccessCode, _hmrcApiBaseUrlConfig.ApiBaseUrl);

        return new ApprenticeshipLevyApiClient(httpclient);
    }
}
