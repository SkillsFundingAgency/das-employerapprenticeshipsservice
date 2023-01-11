namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class DateTimeRegistry : Registry
{
    public DateTimeRegistry()
    {
        Policies.Add<CurrentDatePolicy>();
    }
}