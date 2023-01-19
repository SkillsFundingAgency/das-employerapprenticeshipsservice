using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.EmployerUserRoles.Handlers;
using SFA.DAS.Authorization.Logging;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions
{
    public static class AuthorizationServiceRegistrations
    {
        public static IServiceCollection AddDasAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization<AuthorizationContextProvider>();
            services.AddTransient<DAS.Authorization.Handlers.IAuthorizationHandler, AuthorizationResultLogger>(s => new AuthorizationResultLogger(new AuthorizationHandler(s.GetRequiredService<IEmployerAccountsApiClient>()), s.GetRequiredService<ILogger<AuthorizationResultLogger>>()));
            return services;
        }
    }
}
