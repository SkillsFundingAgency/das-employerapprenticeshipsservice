using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class CommitmentsV2ApiClientServiceRegistrations
{
    public static IServiceCollection AddCommittmentsV2Client(this IServiceCollection services, CommitmentsApiV2ClientConfiguration commitmentsApiV2ClientConfiguration)
    {
        services.AddSingleton(commitmentsApiV2ClientConfiguration);

        services.AddHttpClient<ICommitmentsV2ApiClient, CommitmentsV2ApiClient>();

        return services;
    }
}