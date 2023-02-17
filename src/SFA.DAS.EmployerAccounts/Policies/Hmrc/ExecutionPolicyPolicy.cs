using System.Reflection;
using StructureMap.Pipeline;

namespace SFA.DAS.EmployerAccounts.Policies.Hmrc
{
    public class ExecutionPolicyPolicy : ConfiguredInstancePolicy
    {
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var policies = instance?.Constructor?.GetParameters().Where(x => x.ParameterType == typeof(EmployerAccounts.Policies.Hmrc.ExecutionPolicy)) ?? new ParameterInfo[0];
            foreach (var policyDependency in policies)
            {
                var policyName = policyDependency.GetCustomAttribute<RequiredPolicyAttribute>()?.Name;
                instance?.Dependencies.AddForConstructorParameter(policyDependency, new ReferencedInstance(policyName));
            }
        }
    }
}