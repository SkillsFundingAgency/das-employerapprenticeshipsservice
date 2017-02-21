using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcEmployer;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HmrcService : IHmrcService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ITotpService _totpService;

        public HmrcService(EmployerApprenticeshipsServiceConfiguration configuration, IHttpClientWrapper httpClientWrapper, ITotpService totpService)
        {
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
            _totpService = totpService;
            _httpClientWrapper.BaseUrl = _configuration.Hmrc.BaseUrl;
            _httpClientWrapper.AuthScheme = "Bearer";
            _httpClientWrapper.MediaTypeWithQualityHeaderValueList = new List<MediaTypeWithQualityHeaderValue> {new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json")};
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

            var response = await _httpClientWrapper.SendMessage("", url);

            return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
        }
        
        public async Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef)
        {   
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}";

            return await _httpClientWrapper.Get<EmpRefLevyInformation>(authToken, url);   
        }

        public async Task<string> DiscoverEmpref(string authToken)
        {
            var json = await _httpClientWrapper.Get<EmprefDiscovery>(authToken, "apprenticeship-levy/");

            return json.Emprefs.SingleOrDefault();
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef)
        {
            return await GetLevyDeclarations(empRef, null);
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef,DateTime? fromDate)
        {
            var authToken = await GetOgdAuthenticationToken();

            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/declarations";

            if (fromDate.HasValue)
            {
                url += $"?fromDate={fromDate.Value.ToString("yyyy-MM-dd")}";
            }

            return await _httpClientWrapper.Get<LevyDeclarations>(authToken.AccessToken, url);
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef)
        {
            return await GetEnglishFractions(empRef, null);
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate)
        {
            var authToken = await GetOgdAuthenticationToken();

            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/fractions";

            if (fromDate.HasValue)
            {
                url += $"?fromDate={fromDate.Value.ToString("yyyy-MM-dd")}";
            }

            return await _httpClientWrapper.Get<EnglishFractionDeclarations>(authToken.AccessToken, url);
        }

        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            var authToken = await GetOgdAuthenticationToken();
            const string url = "apprenticeship-levy/fraction-calculation-date";
            return await _httpClientWrapper.Get<DateTime>(authToken.AccessToken, url);
        }

        public async Task<HmrcTokenResponse> GetOgdAuthenticationToken()
        {
            var code = _totpService.GetCode();
            var url = "oauth/token";
            var content = new 
            {
                client_secret= code,
                client_id = _configuration.Hmrc.OgdClientId,
                grant_type = "client_credentials",
                scopes = "read:apprenticeship - levy"
            };
            
            var response = await _httpClientWrapper.SendMessage(content, url);

            return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
        }
    }
}