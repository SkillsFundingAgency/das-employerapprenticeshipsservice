namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class EmployerAccountsAuthorizationRegistry : Registry
{
    public EmployerAccountsAuthorizationRegistry()
    {
        For<IAuthorisationResourceRepository>().Use<AuthorisationResourceRepository>();
    }
}