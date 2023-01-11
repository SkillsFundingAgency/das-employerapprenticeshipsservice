namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class RepositoriesRegistry : Registry
{
    public RepositoriesRegistry()
    {
        For<IUserAccountRepository>().Use<UserAccountRepository>();
    }
}