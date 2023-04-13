﻿using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public class LevyTokenHttpClientMaker: ILevyTokenHttpClientFactory
    {
        private readonly IHmrcApiClientConfiguration _hmrcApiBaseUrlConfig;
        private readonly ITokenServiceApiClient _tokenServiceApiClient;

        public LevyTokenHttpClientMaker(ITokenServiceApiClient tokenServiceApiClient, IHmrcApiClientConfiguration hmrcApiBaseUrlConfig)
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
}
