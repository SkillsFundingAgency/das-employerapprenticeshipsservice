using System;

using NLog;

using Polly;

using SFA.DAS.EAS.Domain.Http;

namespace SFA.DAS.EAS.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class IdamsExecutionPolicy : ExecutionPolicy
    {
        private readonly ILogger _logger;

        public const string Name = "IDAMS Policy";

        public IdamsExecutionPolicy(ILogger logger)
        {
            _logger = logger;

            var tooManyRequestsPolicy = CreateAsyncRetryPolicy<TooManyRequestsException>(9, new TimeSpan(0, 0, 1), OnRetryableFailure);
            var serviceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(4, new TimeSpan(0, 0, 5), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(tooManyRequestsPolicy, serviceUnavailablePolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.Error(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Info(ex, $"Error calling IDAMS - {ex.Message} - Will retry");
        }
    }
}