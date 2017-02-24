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
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HmrcService : IHmrcService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ITokenServiceApiClient _tokenServiceApiClient;


        public HmrcService(EmployerApprenticeshipsServiceConfiguration configuration, IHttpClientWrapper httpClientWrapper, ITokenServiceApiClient tokenServiceApiClient)
        {
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
            _tokenServiceApiClient = tokenServiceApiClient;

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
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);

            var url = $"oauth/token?client_secret={_configuration.Hmrc.ClientSecret}&client_id={_configuration.Hmrc.ClientId}&grant_type=authorization_code&redirect_uri={urlFriendlyRedirectUrl}&code={accessCode}";

            try
            {
                var response = await _httpClientWrapper.SendMessage("", url);

                return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
            }
            catch
            {
                return null;
            }
        }

        public async Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef)
        {
            try
            {
                var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}";

                return await _httpClientWrapper.Get<EmpRefLevyInformation>(authToken, url);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> DiscoverEmpref(string authToken)
        {
            try
            {
                var json = await _httpClientWrapper.Get<EmprefDiscovery>(authToken, "apprenticeship-levy/");

                return json.Emprefs.SingleOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef)
        {
            return await GetLevyDeclarations(empRef, null);
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef, DateTime? fromDate)
        {
            try
            {
                var accessToken = await GetOgdAccessToken();

                var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/declarations";

                if (fromDate.HasValue)
                {
                    url += $"?fromDate={fromDate.Value.ToString("yyyy-MM-dd")}";
                }

                return await _httpClientWrapper.Get<LevyDeclarations>(accessToken, url);
            }
            catch
            {
                return null;
            }
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef)
        {
            return await GetEnglishFractions(empRef, null);
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate)
        {
            try
            {
                var accessToken = await GetOgdAccessToken();

                var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/fractions";

                if (fromDate.HasValue)
                {
                    url += $"?fromDate={fromDate.Value.ToString("yyyy-MM-dd")}";
                }

                return await _httpClientWrapper.Get<EnglishFractionDeclarations>(accessToken, url);
            }
            catch
            {
                return null;
            }
        }

        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            try
            {
                var accessToken = await GetOgdAccessToken();

                const string url = "apprenticeship-levy/fraction-calculation-date";
                return await _httpClientWrapper.Get<DateTime>(accessToken, url);
            }
            catch
            {
                return default(DateTime);
            }
        }

        public async Task<string> GetOgdAccessToken()
        {
            var accessToken = await _tokenServiceApiClient.GetPrivilegedAccessTokenAsync();
            return accessToken.AccessCode;
        }
    }
}