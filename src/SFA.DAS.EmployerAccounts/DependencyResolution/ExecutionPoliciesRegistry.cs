using SFA.DAS.Hmrc.ExecutionPolicy;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class ExecutionPoliciesRegistry : Registry
{
    public ExecutionPoliciesRegistry()
    {
        For<ExecutionPolicy>().Use<HmrcExecutionPolicy>().Named(HmrcExecutionPolicy.Name).SelectConstructor(() => new HmrcExecutionPolicy(null));
        Policies.Add(new ExecutionPolicyPolicy());
    }
}