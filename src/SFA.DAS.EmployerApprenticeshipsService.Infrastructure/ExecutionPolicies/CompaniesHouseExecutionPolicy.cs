using System;
using Polly;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class CompaniesHouseExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "Companies House Policy";

        private readonly ILog _logger;
        private readonly Policy TooManyRequestsPolicy;
        private readonly Policy ServiceUnavailablePolicy;

        public CompaniesHouseExecutionPolicy(ILog logger)
        {
            _logger = logger;

            TooManyRequestsPolicy = CreateAsyncRetryPolicy<TooManyRequestsException>(9, new TimeSpan(0, 0, 1), OnRetryableFailure);
            ServiceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(4, new TimeSpan(0, 0, 5), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(TooManyRequestsPolicy, ServiceUnavailablePolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            var httpException = ex as HttpException;

            //Ignore 404 not found errors
            if (httpException != null && httpException.StatusCode.Equals(404))
            {
                _logger.Info($"Resource could not be found {ex.Message}");
                return default(T);
            }

            _logger.Error(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Info($"Error calling companies house - {ex.Message} - Will retry");
        }

    }
}
