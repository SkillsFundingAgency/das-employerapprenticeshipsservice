using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcEmployer;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HmrcService : IHmrcService
    {
        private readonly ILogger _logger;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public HmrcService(ILogger logger, EmployerApprenticeshipsServiceConfiguration configuration, IHttpClientWrapper httpClientWrapper)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
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

        public async Task<LevyDeclarations> GetLevyDeclarations(string authToken, string empRef)
        {
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/declarations";
            return await _httpClientWrapper.Get<LevyDeclarations>(authToken, url);
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string authToken, string empRef)
        {
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/fractions";
            return await _httpClientWrapper.Get<EnglishFractionDeclarations>(authToken, url);
        }

        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            const string url = "apprenticeship-levy/fraction-calculation-date";
            return await _httpClientWrapper.Get<DateTime>(_configuration.Hmrc.ServerToken, url);
        }
    }
}