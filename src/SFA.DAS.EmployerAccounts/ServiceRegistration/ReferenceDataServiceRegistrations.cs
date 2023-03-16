using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ReferenceData.Api.Client;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ReferenceDataServiceRegistrations
{
    public static IServiceCollection AddReferenceDataApi(this IServiceCollection services)
    {
        services.AddScoped<IReferenceDataService, ReferenceDataService>();
        services.AddScoped<IReferenceDataApiClient>(sp =>
        {
            return new ReferenceDataApiClient(sp.GetService<IReferenceDataApiConfiguration>());
        });

        return services;
    }
}