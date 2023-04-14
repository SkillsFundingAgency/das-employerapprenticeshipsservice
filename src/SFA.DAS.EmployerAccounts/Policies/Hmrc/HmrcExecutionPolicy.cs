using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;

namespace SFA.DAS.EmployerAccounts.Policies.Hmrc;

[PolicyName(Name)]
public class HmrcExecutionPolicy : ExecutionPolicy
{
    public const string Name = "HMRC Policy";

    private readonly ILogger<HmrcExecutionPolicy> _logger;

    public HmrcExecutionPolicy(ILogger<HmrcExecutionPolicy> logger) : this(logger, new TimeSpan(0, 0, 10))
    {
    }

    public HmrcExecutionPolicy(ILogger<HmrcExecutionPolicy> logger, TimeSpan retryWaitTime)
    {
        _logger = logger;

        var tooManyRequestsPolicy = Policy.Handle<ApiHttpException>(ex => ex.HttpCode.Equals(429)).WaitAndRetryForeverAsync(i => retryWaitTime, (ex, ts) => OnRetryableFailure(ex));
        var requestTimeoutPolicy = Policy.Handle<ApiHttpException>(ex => ex.HttpCode.Equals(408)).WaitAndRetryForeverAsync(i => retryWaitTime, (ex, ts) => OnRetryableFailure(ex));
        var serviceUnavailablePolicy = CreateAsyncRetryPolicy<ApiHttpException>(ex => ex.HttpCode.Equals(503), 5, retryWaitTime, OnRetryableFailure);
        var internalServerErrorPolicy = CreateAsyncRetryPolicy<ApiHttpException>(ex => ex.HttpCode.Equals(500), 5, retryWaitTime, OnRetryableFailure);

        RootPolicy = Policy.WrapAsync(tooManyRequestsPolicy, serviceUnavailablePolicy, internalServerErrorPolicy, requestTimeoutPolicy);
    }

    protected override T OnException<T>(Exception ex)
    {
        if (ex is ApiHttpException exception)
        {
            _logger.LogInformation("ApiHttpException - {Ex}", ex);

            switch (exception.HttpCode)
            {
                case 404:
                    _logger.LogInformation("Resource not found - {Ex}", ex);
                    return default;
            }
        }

        _logger.LogError(ex, "Exceeded retry limit - {Ex}", ex);
        throw ex;
    }

    private void OnRetryableFailure(Exception ex)
    {
        _logger.LogInformation("Error calling HMRC - {Ex} - Will retry", ex);
    }
}