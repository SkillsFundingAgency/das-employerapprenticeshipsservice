using System;

namespace SFA.DAS.EAS.Infrastructure.Http.ExecutionPolicies
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PolicyNameAttribute : Attribute
    {
        public PolicyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}