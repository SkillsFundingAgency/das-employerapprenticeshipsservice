namespace SFA.DAS.EmployerAccounts.Attributes;

[AttributeUsage(AttributeTargets.Constructor)]
public class ServiceBusConnectionKeyAttribute : Attribute
{
    public ServiceBusConnectionKeyAttribute(string connectionKey)
    {
                
    }
}