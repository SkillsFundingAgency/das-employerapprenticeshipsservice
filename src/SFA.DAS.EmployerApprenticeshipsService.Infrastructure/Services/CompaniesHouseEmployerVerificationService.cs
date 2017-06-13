using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Employer;
using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class CompaniesHouseEmployerVerificationService : IEmployerVerificationService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly ILog _logger;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ExecutionPolicy _executionPolicy;

        public CompaniesHouseEmployerVerificationService(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, IHttpClientWrapper httpClientWrapper,
            [RequiredPolicy(CompaniesHouseExecutionPolicy.Name)]ExecutionPolicy executionPolicy)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientWrapper = httpClientWrapper;
            _executionPolicy = executionPolicy;
            _httpClientWrapper.AuthScheme = "Basic";
            _httpClientWrapper.BaseUrl = _configuration.CompaniesHouse.BaseUrl;
        }

        public async Task<EmployerInformation> GetInformation(string id)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                _logger.Info($"GetInformation({id})");

                id = id?.ToUpper();

                var result = await _httpClientWrapper.Get<EmployerInformation>(
                    $"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_configuration.CompaniesHouse.ApiKey))}",
                    $"{_configuration.CompaniesHouse.BaseUrl}/company/{id}");
                return result;
            });
        }
    }
}