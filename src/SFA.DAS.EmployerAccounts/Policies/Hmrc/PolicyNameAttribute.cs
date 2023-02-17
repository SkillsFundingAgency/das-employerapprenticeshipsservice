namespace SFA.DAS.EmployerAccounts.Policies.Hmrc;

[AttributeUsage(AttributeTargets.Class)]
public class PolicyNameAttribute : Attribute
{
    public PolicyNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}