using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Events;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IPayeSchemesService, PayeSchemesService>();
        services.AddTransient<IUserAornPayeLockService, UserAornPayeLockService>();
        services.AddTransient<IGenericEventFactory, GenericEventFactory>();
        services.AddTransient<IPayeSchemeEventFactory, PayeSchemeEventFactory>();

        return services;
    }
}
