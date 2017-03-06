using System;
using NLog;
using Polly;
using SFA.DAS.EAS.Domain.Http;

namespace SFA.DAS.EAS.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class CompaniesHouseExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "Companies House Policy";

        private readonly ILogger _logger;
        private readonly Policy TooManyRequestsPolicy;
        private readonly Policy ServiceUnavailablePolicy;

        public CompaniesHouseExecutionPolicy(ILogger logger)
        {
            _logger = logger;

            TooManyRequestsPolicy = CreateAsyncRetryPolicy<TooManyRequestsException>(9, new TimeSpan(0, 0, 1), OnRetryableFailure);
            ServiceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(4, new TimeSpan(0, 0, 5), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(TooManyRequestsPolicy, ServiceUnavailablePolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.Error(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Info(ex, $"Error calling companies house - {ex.Message} - Will retry");
        }

    }
}
