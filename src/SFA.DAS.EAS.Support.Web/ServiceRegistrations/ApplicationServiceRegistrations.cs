using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Web.Services;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IPayeLevyMapper, PayeLevyMapper>();

        services.AddSingleton<IAccountHandler, AccountHandler>();
        services.AddSingleton<IChallengeHandler, ChallengeHandler>();
        services.AddSingleton<IPayeLevySubmissionsHandler, PayeLevySubmissionsHandler>();

        services.AddSingleton<IPayeSchemeObfuscator, PayeSchemeObfuscator>();
        services.AddSingleton<IDatetimeService, DatetimeService>();
        services.AddSingleton<IChallengeService, ChallengeService>();

        return services;
    }
}
