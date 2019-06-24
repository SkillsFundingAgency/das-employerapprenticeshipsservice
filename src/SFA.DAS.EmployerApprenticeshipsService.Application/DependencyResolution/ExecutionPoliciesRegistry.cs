using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ExecutionPoliciesRegistry : Registry
    {
        public ExecutionPoliciesRegistry()
        {
            For<ExecutionPolicy>().Use<HmrcExecutionPolicy>().Named(HmrcExecutionPolicy.Name).SelectConstructor(() => new HmrcExecutionPolicy(null));
            Policies.Add(new ExecutionPolicyPolicy());
        }
    }
}