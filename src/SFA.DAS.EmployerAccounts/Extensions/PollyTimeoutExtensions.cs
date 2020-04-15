using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using Polly;
using Polly.Timeout;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class PollyTimeoutExtensions
    {
        public static AsyncTimeoutPolicy GetTimeoutPolicy(ILog logger)
        {
            return Policy
                .TimeoutAsync(1,
                     (context, timeSpan, retryCount, exception) =>
                {
                    logger.Warn($"Error executing command for method {context.OperationKey} " +
                                $"Reason: {exception.Message}. " +
                                $"Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                    return Task.CompletedTask;
                });
        }
    }
}
