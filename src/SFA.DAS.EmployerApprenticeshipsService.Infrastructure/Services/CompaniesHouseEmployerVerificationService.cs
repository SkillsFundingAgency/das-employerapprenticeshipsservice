using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Employer;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class CompaniesHouseEmployerVerificationService : IEmployerVerificationService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public CompaniesHouseEmployerVerificationService(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger, IHttpClientWrapper httpClientWrapper)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientWrapper = httpClientWrapper;
            _httpClientWrapper.AuthScheme = "Basic";
            _httpClientWrapper.BaseUrl = _configuration.CompaniesHouse.BaseUrl;
        }

        public async Task<EmployerInformation> GetInformation(string id)
        {
            _logger.Info($"GetInformation({id})");

            id = id?.ToUpper();

            var result = await _httpClientWrapper.Get<EmployerInformation>(
                $"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_configuration.CompaniesHouse.ApiKey))}",
                $"{_configuration.CompaniesHouse.BaseUrl}/company/{id}");
            return result;
        }
    }
}