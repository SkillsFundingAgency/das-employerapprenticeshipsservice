using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class PollyPolicyServiceRegistrations
{
    public static IServiceCollection AddPollyPolicy(this IServiceCollection services, EmployerAccountsConfiguration configuration)
    {
        services.AddTransient<IReadOnlyPolicyRegistry<string>>(provider =>
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(PollyPolicyServiceRegistrations)); // Pass in a logger category name
            var policyRegistry = new PolicyRegistry();

            var timeout = Policy
                .TimeoutAsync(TimeSpan.FromMilliseconds(30000), TimeoutStrategy.Pessimistic
                    , (pollyContext, timeSpan, task) =>
                    {
                        logger.LogWarning($"Error executing command for method {pollyContext.ExecutionGuid} " +
                                          $"Reason: {task?.Exception?.Message}. " +
                                          $"Retrying in {timeSpan.Seconds} secs..."
                        );
                        return Task.CompletedTask;
                    }
                );

            policyRegistry.Add(Constants.DefaultServiceTimeout, timeout);

            return policyRegistry;
        });

        return services;
    }
}
