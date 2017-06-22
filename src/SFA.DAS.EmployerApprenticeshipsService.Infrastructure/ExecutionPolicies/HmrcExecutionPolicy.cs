using System;
using Polly;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class HmrcExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "HMRC Policy";

        private readonly ILog _logger;
        private readonly Policy TooManyRequestsPolicy;
        private readonly Policy ServiceUnavailablePolicy;
        private readonly Policy InternalServerErrorPolicy;
        private readonly Policy RequestTimeoutPolicy;

        public HmrcExecutionPolicy(ILog logger)
        {
            _logger = logger;

            TooManyRequestsPolicy = Policy.Handle<TooManyRequestsException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) => OnRetryableFailure(ex));
            RequestTimeoutPolicy = Policy.Handle<RequestTimeOutException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) => OnRetryableFailure(ex));
            ServiceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            InternalServerErrorPolicy = CreateAsyncRetryPolicy<InternalServerErrorException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(TooManyRequestsPolicy, ServiceUnavailablePolicy, InternalServerErrorPolicy, RequestTimeoutPolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.Error(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Info($"Error calling HMRC - {ex.Message} - Will retry");
        }
    }
}
