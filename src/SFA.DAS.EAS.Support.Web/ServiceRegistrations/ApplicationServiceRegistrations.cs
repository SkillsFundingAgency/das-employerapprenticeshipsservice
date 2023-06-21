﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Support.Web.Services;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

[ExcludeFromCodeCoverage]
public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IPayRefHashingService, PayRefHashingService>(sp =>
        {
            var hashConfig = sp.GetService<IEasSupportConfiguration>().HashingService;
            return new PayRefHashingService(hashConfig.AllowedCharacters, hashConfig.Hashstring);
        });

        services.AddSingleton<IAccountHandler, AccountHandler>();
        services.AddSingleton<IChallengeService, ChallengeService>();
        services.AddSingleton<IDatetimeService, DatetimeService>();
        services.AddSingleton<IChallengeHandler, ChallengeHandler>();
        services.AddSingleton<IPayeLevySubmissionsHandler, PayeLevySubmissionsHandler>();
        services.AddSingleton<IPayeLevyMapper, PayeLevyMapper>();
        services.AddSingleton<IPayeSchemeObfuscator, PayeSchemeObfuscator>();

        return services;
    }
}
