﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerAccounts.Infrastructure.Data;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class HmrcServiceRegistrations
{
    public static IServiceCollection AddHmrc(this IServiceCollection services, IHmrcConfiguration hmrcConfiguration)
    {
        services.AddSingleton<IHmrcService, HmrcService>();
        services.AddSingleton<IHttpResponseLogger, HttpResponseLogger>();
        services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
        services.AddSingleton<IAzureAdAuthenticationService, AzureAdAuthenticationService>();
        services.AddSingleton<IHttpClientWrapper, Infrastructure.Data.HttpClientWrapper>();

        return services;
    }
}