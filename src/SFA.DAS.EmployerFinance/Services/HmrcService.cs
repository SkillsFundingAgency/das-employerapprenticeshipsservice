using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using HMRC.ESFA.Levy.Api.Client;
using Newtonsoft.Json;
using SFA.DAS.Caches;
using SFA.DAS.Http;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.HmrcLevy;
using SFA.DAS.ExecutionPolicies;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.ActiveDirectory;

namespace SFA.DAS.EmployerFinance.Services
{
    public class HmrcService : IHmrcService
    {
        private readonly EmployerFinanceConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IApprenticeshipLevyApiClient _apprenticeshipLevyApiClient;
        private readonly ITokenServiceApiClient _tokenServiceApiClient;
        private readonly ExecutionPolicy _executionPolicy;
        private readonly IInProcessCache _inProcessCache;
        private readonly IAzureAdAuthenticationService _azureAdAuthenticationService;


        public HmrcService(
            EmployerFinanceConfiguration configuration, 
            IHttpClientWrapper httpClientWrapper,
            IApprenticeshipLevyApiClient apprenticeshipLevyApiClient,
            ITokenServiceApiClient tokenServiceApiClient, 
            [RequiredPolicy(HmrcExecutionPolicy.Name)] ExecutionPolicy executionPolicy,
            IInProcessCache inProcessCache, 
            IAzureAdAuthenticationService azureAdAuthenticationService)
        {
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
            _apprenticeshipLevyApiClient = apprenticeshipLevyApiClient;
            _tokenServiceApiClient = tokenServiceApiClient;
            _executionPolicy = executionPolicy;
            _inProcessCache = inProcessCache;
            _azureAdAuthenticationService = azureAdAuthenticationService;

            _httpClientWrapper.BaseUrl = _configuration.Hmrc.BaseUrl;
            _httpClientWrapper.AuthScheme = "Bearer";
            _httpClientWrapper.MediaTypeWithQualityHeaderValueList = new List<MediaTypeWithQualityHeaderValue> { new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json") };
        }

        public string GenerateAuthRedirectUrl(string redirectUrl)
        {
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
            return $"{_configuration.Hmrc.BaseUrl}oauth/authorize?response_type=code&client_id={_configuration.Hmrc.ClientId}&scope={_configuration.Hmrc.Scope}&redirect_uri={urlFriendlyRedirectUrl}";
        }

        public async Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var requestParams = new
                {
                    client_secret = _configuration.Hmrc.ClientSecret,
                    client_id = _configuration.Hmrc.ClientId,
                    grant_type = "authorization_code",
                    redirect_uri = redirectUrl,
                    code = accessCode
                };

                var response = await _httpClientWrapper.SendMessage(requestParams, "oauth/token");

                return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
            });
        }

        public async Task<HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef)
        {
            return await _executionPolicy.ExecuteAsync(async () => await _apprenticeshipLevyApiClient.GetEmployerDetails(authToken, empRef));
        }

        public async Task<HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation> GetEmprefInformation(string empRef)
        {
            var accessToken =  await _executionPolicy.ExecuteAsync(async () => await GetOgdAccessToken());

            return await GetEmprefInformation(accessToken, empRef);
        }

        public async Task<string> DiscoverEmpref(string authToken)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var response = await _apprenticeshipLevyApiClient.GetAllEmployers(authToken);

                if (response == null)
                    return string.Empty;

                return response.Emprefs.SingleOrDefault();
            });
        }

        public async Task<HMRC.ESFA.Levy.Api.Types.LevyDeclarations> GetLevyDeclarations(string empRef)
        {
            return await GetLevyDeclarations(empRef, null);
        }

        public async Task<HMRC.ESFA.Levy.Api.Types.LevyDeclarations> GetLevyDeclarations(string empRef, DateTime? fromDate)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var accessToken = await GetOgdAccessToken();

                var earliestDate = new DateTime(2017, 04, 01);
                if (!fromDate.HasValue || fromDate.Value < earliestDate)
                {
                    fromDate = earliestDate;
                }

                return await _apprenticeshipLevyApiClient.GetEmployerLevyDeclarations(accessToken, empRef, fromDate);
            });
        }

        public async Task<HMRC.ESFA.Levy.Api.Types.EnglishFractionDeclarations> GetEnglishFractions(string empRef)
        {
            return await GetEnglishFractions(empRef, null);
        }

        public async Task<HMRC.ESFA.Levy.Api.Types.EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var accessToken = await GetOgdAccessToken();

                return await _apprenticeshipLevyApiClient.GetEmployerFractionCalculations(accessToken, empRef, fromDate);
            });
        }

        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            var hmrcLatestUpdateDate = _inProcessCache.Get<DateTime?>("HmrcFractionLastCalculatedDate");
            if (hmrcLatestUpdateDate == null)
            {
                return await _executionPolicy.ExecuteAsync(async () =>
                {
                    var accessToken = await GetOgdAccessToken();

                    hmrcLatestUpdateDate =  await _apprenticeshipLevyApiClient.GetLastEnglishFractionUpdate(accessToken);

                    if (hmrcLatestUpdateDate != null)
                    {
                        _inProcessCache.Set("HmrcFractionLastCalculatedDate", hmrcLatestUpdateDate.Value,new TimeSpan(0,0,30,0));
                    }

                    return hmrcLatestUpdateDate.Value;
                });
            }
            return hmrcLatestUpdateDate.Value;
        }

        public async Task<string> GetOgdAccessToken()
        {
            if (_configuration.Hmrc.UseHiDataFeed)
            {
                var accessToken =
                    await _azureAdAuthenticationService.GetAuthenticationResult(_configuration.Hmrc.ClientId,
                            _configuration.Hmrc.AzureAppKey, _configuration.Hmrc.AzureResourceId,
                            _configuration.Hmrc.AzureTenant);

                return accessToken;
            }
            else
            {
                var accessToken = await _tokenServiceApiClient.GetPrivilegedAccessTokenAsync();
                return accessToken.AccessCode;
            }
            
        }
    }
}