using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Cookies;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.GovUK.Auth.Services;
using EmployerClaims = SFA.DAS.EAS.Application.Infrastructure.EmployerClaims;

namespace SFA.DAS.EAS.Web.StartupExtensions;

public static class AuthenticationServiceRegistrations
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddTransient<IEmployerAccountAuthorisationHandler, EmployerAccountAuthorisationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountAllRolesAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerUsersIsOutsideAccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountOwnerAuthorizationHandler>();
        
        services.AddTransient<ICustomClaims, EmployerAccountPostAuthenticationClaimsHandler>();    

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