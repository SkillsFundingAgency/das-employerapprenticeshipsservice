using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using NLog;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;
using SFA.DAS.EAS.Infrastructure.Models;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class IdamsEmailServiceWrapper
    {

        private readonly ILogger _logger;

        private readonly CommitmentNotificationConfiguration _configuration;

        private readonly IHttpClientWrapper _httpClientWrapper;

        private readonly ExecutionPolicy _executionPolicy;

        public IdamsEmailServiceWrapper(
            ILogger logger,
            EmployerApprenticeshipsServiceConfiguration configuration,
            IHttpClientWrapper httpClient,
            [RequiredPolicy(IdamsExecutionPolicy.Name)]ExecutionPolicy executionPolicy)
        {
            _logger = logger;
            _configuration = configuration.CommitmentNotification;
            _httpClientWrapper = httpClient;
            _executionPolicy = executionPolicy;
        }

        public virtual async Task<List<string>> GetEmailsAsync(long ukprn)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.DasUserRoleId, ukprn);
            _logger.Info($"Getting 'DAS' emails for provider {ukprn}");
            var result = await GetString(url);
            return ParseIdamsResult(result, ukprn);
        }

        public virtual async Task<List<string>> GetSuperUserEmailsAsync(long ukprn)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.SuperUserRoleId, ukprn);
            _logger.Info($"Getting 'super user' emails for provider {ukprn}");
            var result = GetString(url);
            return ParseIdamsResult(await result, ukprn);
        }

        private List<string> ParseIdamsResult(string jsonResult, long ukprn)
        {
            try
            {
                var userReponses =
                    JObject.Parse(jsonResult)
                        .SelectToken("result")
                        .ToObject<IEnumerable<UserResponse>>();
                return userReponses.SelectMany(m => m.Emails).ToList();
            }
            catch (Exception exception)
            {
                _logger.Info($"Result: {jsonResult}");
                _logger.Error(
                    exception,
                    $"Not possible to parse result to {typeof(UserResponse)} for provider: {ukprn}");
            }

            return new List<string>();
        }

        private async Task<string> GetString(string url)
        {
            var result = string.Empty;

            await _executionPolicy.ExecuteAsync(
                async () =>
                    {
                        result = await _httpClientWrapper.GetString(url);
                    });
            return result;
        }
    }
}
