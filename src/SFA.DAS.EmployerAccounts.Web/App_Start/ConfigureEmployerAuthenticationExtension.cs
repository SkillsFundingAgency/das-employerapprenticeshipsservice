using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.EmployerUsers;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web;

public static class ConfigureEmployerAuthenticationExtension
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddTransient<IEmployerAccountAuthorisationHandler, EmployerAccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();
        services.AddTransient<IEmployerAccountService, EmployerAccountService>();
    }
}
