using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class PollyPolicyRegistry : Registry
{
    public PollyPolicyRegistry()
    {
        For<IReadOnlyPolicyRegistry<string>>().Use(c => GetPolicyRegistry(c));
    }

    private PolicyRegistry GetPolicyRegistry(IContext context)
    {
        var logger = context.GetInstance<ILog>();
        var config = context.GetInstance<EmployerAccountsConfiguration>();
        var policyRegistry = new PolicyRegistry();
        var timeout = Policy
            .TimeoutAsync(TimeSpan.FromMilliseconds(config.DefaultServiceTimeoutMilliseconds), TimeoutStrategy.Pessimistic
                , (pollyContext, timeSpan, task) =>
                {
                    logger.Warn($"Error executing command for method {pollyContext.ExecutionKey} " +
                                $"Reason: {task?.Exception?.Message}. " +
                                $"Retrying in {timeSpan.Seconds} secs..."
                    );
                    return Task.CompletedTask;
                }
            );
        policyRegistry.Add(Constants.DefaultServiceTimeout, timeout);
        return policyRegistry;
    }
}