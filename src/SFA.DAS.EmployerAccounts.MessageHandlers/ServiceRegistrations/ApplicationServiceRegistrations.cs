using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.Encoding;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IGenericEventFactory, GenericEventFactory>();
        services.AddSingleton<IAccountEventFactory, AccountEventFactory>();
        services.AddSingleton<IEncodingService, EncodingService>();
        services.AddSingleton<IEventsApiClientConfiguration>(cfg => cfg.GetService<EmployerAccountsConfiguration>().EventsApi);
        services.AddTransient<IAzureClientCredentialHelper, AzureClientCredentialHelper>();

        return services;
    }
}