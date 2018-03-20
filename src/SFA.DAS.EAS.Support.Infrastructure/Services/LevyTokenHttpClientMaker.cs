using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.TokenService.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public class LevyTokenHttpClientMaker: ILevyTokenHttpClientFactory
    {
        private readonly IHmrcApiBaseUrlConfig _hmrcApiBaseUrlConfig;
        private readonly ITokenServiceApiClientConfiguration _levySubmissionsApiConfiguration;

        public LevyTokenHttpClientMaker(ITokenServiceApiClientConfiguration levySubmissionsApiConfiguration, IHmrcApiBaseUrlConfig hmrcApiBaseUrlConfig)
        {
            _hmrcApiBaseUrlConfig = hmrcApiBaseUrlConfig;
            _levySubmissionsApiConfiguration = levySubmissionsApiConfiguration;
        }

        public async  Task<IApprenticeshipLevyApiClient> GetLevyHttpClient()
        {
            var tokenService = new TokenServiceApiClient(_levySubmissionsApiConfiguration);
            var tokenResult = await tokenService.GetPrivilegedAccessTokenAsync();
            var httpclient = ApprenticeshipLevyApiClient.CreateHttpClient(tokenResult.AccessCode, _hmrcApiBaseUrlConfig.HmrcApiBaseUrl);

            return new ApprenticeshipLevyApiClient(httpclient);
        }
    }
}
