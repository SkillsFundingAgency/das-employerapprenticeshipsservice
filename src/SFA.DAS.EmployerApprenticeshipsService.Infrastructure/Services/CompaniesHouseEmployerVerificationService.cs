using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class CompaniesHouseEmployerVerificationService : IEmployerVerificationService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IManagedCompanyLookupService _managedCompanyLookupService;

        public CompaniesHouseEmployerVerificationService(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger,IManagedCompanyLookupService managedCompanyLookupService)
        {
            _configuration = configuration;
            _logger = logger;
            _managedCompanyLookupService = managedCompanyLookupService;
        }

        public async Task<EmployerInformation> GetInformation(string id)
        {
            _logger.Info($"GetInformation({id})");

            id = id.ToUpper();

            if (_configuration.CompaniesHouse.UseManagedList)
            {
                var companies = _managedCompanyLookupService.GetCompanies();
                var company = companies?.Data.SingleOrDefault(c => c.CompanyNumber.Equals(id, StringComparison.CurrentCultureIgnoreCase));
                if (company != null)
                {
                    _logger.Info($"Company {id} returned via managed lookup service.");
                    return company;
                }
            }

            var webClient = new WebClient();
            
            webClient.Headers.Add($"Authorization: Basic {_configuration.CompaniesHouse.ApiKey}");
            

            try
            {
                var result = await webClient.DownloadStringTaskAsync($"https://api.companieshouse.gov.uk/company/{id}");

                return JsonConvert.DeserializeObject<EmployerInformation>(result);
            }
            catch (WebException ex)
            {
                _logger.Error(ex, "There was a problem with the call to Companies House");
            }

            return null;
        }
    }
}