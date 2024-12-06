using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Cookies;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EAS.Web.StartupExtensions;

public static class AuthenticationServiceRegistrations
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddTransient<IEmployerAccountAuthorisationHandler, EmployerAccountAuthorisationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountAllRolesAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerUsersIsOutsideAccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountOwnerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>(); //TODO remove after gov login enabled

        services
            .AddAuthorizationBuilder()
            .AddPolicy(
                PolicyNames.HasUserAccount
                , policy =>
                {
                    policy.RequireClaim(EmployerClaims.IdamsUserIdClaimTypeIdentifier);
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AccountActiveRequirement());
                });

        return services;
    }
}