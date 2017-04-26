using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcEmployer;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HmrcService : IHmrcService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ITokenServiceApiClient _tokenServiceApiClient;
        private readonly ExecutionPolicy _executionPolicy;
        private readonly ICacheProvider _cacheProvider;


        public HmrcService(EmployerApprenticeshipsServiceConfiguration configuration, IHttpClientWrapper httpClientWrapper, ITokenServiceApiClient tokenServiceApiClient, [RequiredPolicy(HmrcExecutionPolicy.Name)] ExecutionPolicy executionPolicy, ICacheProvider cacheProvider)
        {
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
            _tokenServiceApiClient = tokenServiceApiClient;
            _executionPolicy = executionPolicy;
            _cacheProvider = cacheProvider;

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

        public async Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}";

                return await _httpClientWrapper.Get<EmpRefLevyInformation>(authToken, url);
            });
        }

        public async Task<EmpRefLevyInformation> GetEmprefInformation(string empRef)
        {
            var accessToken =  await _executionPolicy.ExecuteAsync(async () => await GetOgdAccessToken());

            return await GetEmprefInformation(accessToken, empRef);
        }

        public async Task<string> DiscoverEmpref(string authToken)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var json = await _httpClientWrapper.Get<EmprefDiscovery>(authToken, "apprenticeship-levy/");

                return json.Emprefs.SingleOrDefault();
            });
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef)
        {
            return await GetLevyDeclarations(empRef, null);
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef, DateTime? fromDate)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var accessToken = await GetOgdAccessToken();

                var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/declarations?fromDate=";

                var defaultFromDate = "2017-04-01";

                if (fromDate.HasValue)
                {
                    if (fromDate.Value >= new DateTime(2017, 04, 01))
                    {
                        url += $"{fromDate.Value.ToString("yyyy-MM-dd")}";
                    }
                    else
                    {
                        url += defaultFromDate;
                    }
                }
                else
                {
                    url += defaultFromDate;
                }

                return await _httpClientWrapper.Get<LevyDeclarations>(accessToken, url);
            });
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef)
        {
            return await GetEnglishFractions(empRef, null);
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var accessToken = await GetOgdAccessToken();

                var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/fractions";

                if (fromDate.HasValue)
                {
                    url += $"?fromDate={fromDate.Value.ToString("yyyy-MM-dd")}";
                }

                return await _httpClientWrapper.Get<EnglishFractionDeclarations>(accessToken, url);
            });
        }

        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            var hmrcLatestUpdateDate = _cacheProvider.Get<DateTime?>("HmrcFractionLastCalculatedDate");
            if (hmrcLatestUpdateDate == null)
            {
                return await _executionPolicy.ExecuteAsync(async () =>
                {
                    var accessToken = await GetOgdAccessToken();

                    const string url = "apprenticeship-levy/fraction-calculation-date";
                    hmrcLatestUpdateDate = await _httpClientWrapper.Get<DateTime>(accessToken, url);

                    if (hmrcLatestUpdateDate != null)
                    {
                        _cacheProvider.Set("HmrcFractionLastCalculatedDate", hmrcLatestUpdateDate.Value,new TimeSpan(1,0,0,0));
                    }

                    return hmrcLatestUpdateDate.Value;
                });
            }
            return hmrcLatestUpdateDate.Value;
        }

        public async Task<string> GetOgdAccessToken()
        {
            var accessToken = await _tokenServiceApiClient.GetPrivilegedAccessTokenAsync();
            return accessToken.AccessCode;
        }
    }
}