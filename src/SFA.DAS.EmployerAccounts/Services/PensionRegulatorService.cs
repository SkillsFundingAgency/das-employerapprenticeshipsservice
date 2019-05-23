using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class PensionRegulatorService : IPensionRegulatorService
    {
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly IHttpService _httpService;    
        private readonly ILog _log;

        public PensionRegulatorService(
            IHttpServiceFactory httpServiceFactory,
            EmployerAccountsConfiguration configuration,
            ILog log)
        {           
            _configuration = configuration;
            _log = log;
            _httpService = httpServiceFactory.Create(
                configuration.PensionRegulator.ClientId,
                configuration.PensionRegulator.ClientSecret,
                configuration.PensionRegulator.IdentifierUri,
                configuration.PensionRegulator.Tenant
            );
        }  

        public async Task<IEnumerable<Organisation>> GetOrgansiationsByPayeRef(string payeRef)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}GetByPayeRef?payeRef={HttpUtility.UrlEncode(payeRef)}";

            var json = await _httpService.GetAsync(url, false);
            return json == null ? null : JsonConvert.DeserializeObject<IEnumerable<Organisation>>(json);
        }

        private string GetBaseUrl()
        {
            var baseUrl = _configuration.PensionRegulator.BaseUrl.EndsWith("/")
                ? _configuration.PensionRegulator.BaseUrl
                : _configuration.PensionRegulator.BaseUrl + "/";

            return baseUrl;
        }
    }
}
