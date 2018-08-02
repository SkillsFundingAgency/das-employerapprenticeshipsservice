using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.ExecutionPolicies;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ExecutionPoliciesRegistry : Registry
    {
        public ExecutionPoliciesRegistry()
        {
            For<ExecutionPolicy>().Use<CompaniesHouseExecutionPolicy>().Named(CompaniesHouseExecutionPolicy.Name);
            For<ExecutionPolicy>().Use<HmrcExecutionPolicy>().Named(HmrcExecutionPolicy.Name);
            For<ExecutionPolicy>().Use<IdamsExecutionPolicy>().Named(IdamsExecutionPolicy.Name);
            Policies.Add(new ExecutionPolicyPolicy());
        }
    }
}