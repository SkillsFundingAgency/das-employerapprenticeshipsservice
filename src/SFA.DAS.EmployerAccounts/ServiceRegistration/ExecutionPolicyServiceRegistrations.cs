using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Policies.Hmrc;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ExecutionPolicyServiceRegistrations
{
    public static IServiceCollection AddExecutionPolicies(this IServiceCollection services)
    {
        services.AddTransient<ExecutionPolicy, HmrcExecutionPolicy>();

        return services;
    }
}