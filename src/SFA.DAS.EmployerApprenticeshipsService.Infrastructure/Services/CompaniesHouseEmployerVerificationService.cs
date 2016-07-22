using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class CompaniesHouseEmployerVerificationService : IEmployerVerificationService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        

        public CompaniesHouseEmployerVerificationService(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EmployerInformation> GetInformation(string id)
        {
            Logger.Info($"GetInformation({id})");

            var webClient = new WebClient();
            
            webClient.Headers.Add($"Authorization: Basic {_configuration.CompaniesHouse.ApiKey}");
            try
            {
                var result = await webClient.DownloadStringTaskAsync($"https://api.companieshouse.gov.uk/company/{id}");

                return JsonConvert.DeserializeObject<EmployerInformation>(result);
            }
            catch (WebException ex)
            {
                Logger.Error(ex, "There was a problem with the call to Companies House");
            }

            return null;
        }
    }
}