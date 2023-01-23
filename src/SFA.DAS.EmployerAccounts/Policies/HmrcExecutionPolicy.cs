using Microsoft.Extensions.Logging;
using Polly;
using SFA.DAS.Hmrc.ExecutionPolicy;
using SFA.DAS.Hmrc.Http;

namespace SFA.DAS.EmployerAccounts.Policies
{

    [PolicyName(Name)]
    public class HmrcExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "HMRC Policy";

        private readonly ILogger<HmrcExecutionPolicy> _logger;
        private readonly Policy _tooManyRequestsPolicy;
        private readonly Policy _serviceUnavailablePolicy;
        private readonly Policy _internalServerErrorPolicy;
        private readonly Policy _requestTimeoutPolicy;
        private readonly Policy _unauthorizedPolicy;

        public HmrcExecutionPolicy(ILogger<HmrcExecutionPolicy> logger)
        {
            _logger = logger;

            _tooManyRequestsPolicy = Policy.Handle<HttpException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) =>
            {
                if (((HttpException)ex).StatusCode == 429)
                {
                    OnRetryableFailure(ex, 429, "Rate limit has been reached");
                }
            });
            _requestTimeoutPolicy = Policy.Handle<HttpException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) =>
            {
                if (((HttpException)ex).StatusCode == 408)
                {
                    OnRetryableFailure(ex, 408, "Request has time out");
                }
            });
            _serviceUnavailablePolicy = CreateAsyncRetryPolicy<HttpException>(5, new TimeSpan(0, 0, 10), (ex) =>
            {
                if (((HttpException)ex).StatusCode == 503)
                {
                    OnRetryableFailure(ex, 503, "Service is unavailable");
                }
            });
            _internalServerErrorPolicy = CreateAsyncRetryPolicy<HttpException>(5, new TimeSpan(0, 0, 10), (ex) =>
            {
                if (((HttpException)ex).StatusCode == 500)
                {
                    OnRetryableFailure(ex, 500, "Internal server error");
                }
            });
            _unauthorizedPolicy =
                CreateAsyncRetryPolicy<HttpException>(5, new TimeSpan(0, 0, 10), (ex) =>
                {
                    if (((HttpException)ex).StatusCode == 401)
                    {
                        OnRetryableFailure(ex, 401, ex.Message);
                    }
                });
            RootPolicy = Policy.WrapAsync(_tooManyRequestsPolicy, _serviceUnavailablePolicy, _internalServerErrorPolicy, _requestTimeoutPolicy, _unauthorizedPolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.LogError(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex, int statusCode, string message)
        {
            _logger.LogInformation($"Error calling HMRC - {ex.Message} - Will retry");
        }
    }
}
