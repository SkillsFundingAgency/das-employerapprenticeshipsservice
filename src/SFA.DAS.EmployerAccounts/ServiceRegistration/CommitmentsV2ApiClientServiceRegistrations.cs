using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class CommitmentsV2ApiClientServiceRegistrations
{
    public static IServiceCollection AddCommittmentsV2Client(this IServiceCollection services)
    {
        services.AddSingleton<ICommitmentsV2ApiClient, CommitmentsV2ApiClient>();

        services.AddHttpClient<ICommitmentsV2ApiClient, CommitmentsV2ApiClient>()
            .AddHttpMessageHandler<RequestIdMessageRequestHandler>()
            .AddHttpMessageHandler<SessionIdMessageRequestHandler>();

        return services;
    }
}