using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

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