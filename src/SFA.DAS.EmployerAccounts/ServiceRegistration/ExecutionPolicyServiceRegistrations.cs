using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Hmrc.ExecutionPolicy;
using HmrcExecutionPolicy = SFA.DAS.EmployerAccounts.Policies.HmrcExecutionPolicy;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ExecutionPolicyServiceRegistrations
{
    public static IServiceCollection AddExecutionPolicies(this IServiceCollection services)
    {
        services.AddTransient<ExecutionPolicy, HmrcExecutionPolicy>();

        return services;
    }
}