using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class PensionRegulatorService : IPensionRegulatorService
    {
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly IHttpService _httpService;    
       
        public PensionRegulatorService(
            IHttpServiceFactory httpServiceFactory,
            EmployerAccountsConfiguration configuration)
        {           
            _configuration = configuration;         
            _httpService = httpServiceFactory.Create(
                configuration.PensionRegulatorApi.ClientId,
                configuration.PensionRegulatorApi.ClientSecret,
                configuration.PensionRegulatorApi.IdentifierUri,
                configuration.PensionRegulatorApi.Tenant
            );
        }  

        public async Task<IEnumerable<Organisation>> GetOrgansiationsByPayeRef(string payeRef)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/pensionsregulator?payeRef={HttpUtility.UrlEncode(payeRef)}";

            var json = await _httpService.GetAsync(url, false);
            return json == null ? null : JsonConvert.DeserializeObject<IEnumerable<Organisation>>(json);
        }

        private string GetBaseUrl()
        {
            var baseUrl = _configuration.PensionRegulatorApi.BaseUrl.EndsWith("/")
                ? _configuration.PensionRegulatorApi.BaseUrl
                : _configuration.PensionRegulatorApi.BaseUrl + "/";

            return baseUrl;
        }
    }
}
