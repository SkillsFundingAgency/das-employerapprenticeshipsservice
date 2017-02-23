using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using NLog;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Models;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class IdamsEmailServiceWrapper
    {

        private readonly ILogger _logger;

        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        private readonly IHttpClientWrapper _httpClientWrapper;

        public IdamsEmailServiceWrapper(
            ILogger logger, 
            EmployerApprenticeshipsServiceConfiguration configuration,
            IHttpClientWrapper httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientWrapper = httpClient;
        }

        public virtual async Task<List<string>> GetEmailsAsync(long ukprn)
        {
            var url = string.Format(_configuration.CommitmentNotification.IdamsListUsersUrl, ukprn);
            var result = await _httpClientWrapper.GetString(url);

            try
            {
                var re =
                    JObject.Parse(result)
                        .SelectToken("result")
                        .ToObject<UserResponse>();
                return re?.Emails ?? new List<string>();
            }
            catch (Exception exception)
            {
                _logger.Info($"Result: {result}");
                _logger.Error(
                    exception,
                    $"Not possible to parse result to {typeof(UserResponse)} for provider: {ukprn}");
            }
            return new List<string>();
        }
    }
}
