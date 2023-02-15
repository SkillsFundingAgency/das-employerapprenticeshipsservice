namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;

public static class ServiceLocator
{
    private static IContainer _container;

    public static void Initialize(IContainer container)
    {
        _container = container;
    }

    public static T GetInstance<T>()
    {
        return _container.GetInstance<T>();
    }
}