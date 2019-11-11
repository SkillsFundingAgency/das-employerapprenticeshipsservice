using SFA.DAS.Hmrc.ExecutionPolicy;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
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