using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class CommitmentsV2ApiClient : ApiClientBase, ICommitmentsV2ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CommitmentsApiV2ClientConfiguration _config;
        private readonly ILogger<CommitmentsV2ApiClient> _logger;

        public CommitmentsV2ApiClient(HttpClient httpClient, CommitmentsApiV2ClientConfiguration config, ILogger<CommitmentsV2ApiClient> logger) : base(httpClient)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<GetApprenticeshipResponse> GetApprenticeship(long apprenticeshipId)
        {
            var url = $"{BaseUrl()}api/apprenticeships/{apprenticeshipId}";
            _logger.LogInformation($"Getting GetApprenticeship {url}");
            var response = JsonConvert.DeserializeObject<GetApprenticeshipResponse>(await GetAsync(url));
            return response;
        }

        private string BaseUrl()
        {
            if (_config.ApiBaseUrl.EndsWith("/"))
            {
                return _config.ApiBaseUrl;
            }

            return _config.ApiBaseUrl + "/";
        }
    }
}
