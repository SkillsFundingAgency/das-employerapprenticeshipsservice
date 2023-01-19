using SFA.DAS.Hmrc.ExecutionPolicy;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ExecutionPolicyServiceRegistrations
{
    public static IServiceCollection AddExectionPolicies(this IServiceCollection services)
    {
        services.AddTransient<ExecutionPolicy, HmrcExecutionPolicy>();

        return services;
    }
}