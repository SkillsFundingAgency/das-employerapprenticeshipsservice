using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class CompaniesHouseEmployerVerificationService : IEmployerVerificationService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _apiKey;

        public CompaniesHouseEmployerVerificationService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<EmployerInformation> GetInformation(string id)
        {
            Logger.Info($"GetInformation({id})");

            var webClient = new WebClient();

            webClient.Headers.Add($"Authorization: Basic {_apiKey}");
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