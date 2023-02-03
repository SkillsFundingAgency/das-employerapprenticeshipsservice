using SFA.DAS.Authorization.Handlers;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class EmployerAccountsAuthorizationRegistry : Registry
{
    public EmployerAccountsAuthorizationRegistry()
    {
        For<IAuthorizationHandler>().Add<EmployerFeatureAuthorizationHandler>();
        For<IAuthorisationResourceRepository>().Use<AuthorisationResourceRepository>();
    }
}