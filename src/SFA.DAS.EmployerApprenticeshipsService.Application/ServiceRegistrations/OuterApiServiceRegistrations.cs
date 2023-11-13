using System;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Application.Contracts.OuterApi;
using SFA.DAS.EAS.Application.Infrastructure.OuterApi;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.ServiceRegistrations;

public static class OuterApiServiceRegistrations
{
    public static IServiceCollection AddOuterApiClient(this IServiceCollection services, OuterApiConfiguration outerApiConfiguration)
    {
        services.AddHttpClient<IOuterApiClient, OuterApiClient>(x =>
        {
            x.BaseAddress = new Uri(outerApiConfiguration.BaseUrl);
        });

        return services;
    }
}