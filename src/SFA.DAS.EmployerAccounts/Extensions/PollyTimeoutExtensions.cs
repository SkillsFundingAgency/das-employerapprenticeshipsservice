using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using Polly;
using Polly.Timeout;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class PollyTimeoutExtensions
    {
        public static TimeoutPolicy GetTimeoutPolicy(ILog logger)
        {
            return Policy.TimeoutAsync(1);
            //.TimeoutAsync(1,TimeoutStrategy.Pessimistic,(context, timeSpan, task) =>
            //{
            //    logger.Warn($"Error executing command for method {context.ExecutionKey} " +
            //                $"Reason: {task?.Exception?.Message}. " +
            //                $"Retrying in {timeSpan.Seconds} secs..."
            //                );
            //    return Task.CompletedTask;
            //}
            //);
        }
    }
}
