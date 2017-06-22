using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;
using SFA.DAS.EAS.Infrastructure.Models;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class IdamsEmailServiceWrapper
    {

        private readonly ILog _logger;

        private readonly CommitmentNotificationConfiguration _configuration;

        private readonly IHttpClientWrapper _httpClientWrapper;

        private readonly ExecutionPolicy _executionPolicy;

        public IdamsEmailServiceWrapper(
            ILog logger,
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
            var result = await GetString(url, _configuration.ClientToken);
            return ParseIdamsResult(result, ukprn);
        }

        public virtual async Task<List<string>> GetSuperUserEmailsAsync(long ukprn)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.SuperUserRoleId, ukprn);
            _logger.Info($"Getting 'super user' emails for provider {ukprn}");
            var result = GetString(url, _configuration.ClientToken);
            return ParseIdamsResult(await result, ukprn);
            
        }

        private List<string> ParseIdamsResult(string jsonResult, long ukprn)
        {
            try
            {
                var result = JObject.Parse(jsonResult).SelectToken("result");

                if (result.Type == JTokenType.Array)
                {
                    var items = result.ToObject<IEnumerable<UserResponse>>();
                    return items.SelectMany(m => m.Emails).ToList();
                }

                var item = result.ToObject<UserResponse>();
                return item?.Emails ?? new List<string>(0);
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

        private async Task<string> GetString(string url, string accessToken)
        {
            var result = string.Empty;
            try
            {
                await _executionPolicy.ExecuteAsync(
                    async () =>
                        {
                            _httpClientWrapper.AuthScheme = "Bearer";
                            result = await _httpClientWrapper.GetString(url, accessToken);
                        });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting idams emails");
            }
            return result;
        }
    }
}

