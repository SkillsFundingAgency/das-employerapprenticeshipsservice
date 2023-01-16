namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class MediatorRegistry : Registry
{
    public MediatorRegistry()
    {
        For<IMediator>().Use<Mediator>();
    }
}