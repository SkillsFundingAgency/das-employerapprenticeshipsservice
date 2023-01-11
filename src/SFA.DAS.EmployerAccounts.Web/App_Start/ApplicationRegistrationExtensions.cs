using SFA.DAS.Audit.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web;

public static class ApplicationRegistrationExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, EmployerAccountsConfiguration configuration)
    {
        services.AddTransient<IRestClientFactory, RestClientFactory>();
        services.AddTransient<IRestServiceFactory, RestServiceFactory>();
        services.AddTransient<IHttpServiceFactory, HttpServiceFactory>();
        services.AddTransient<IUserAornPayeLockService, UserAornPayeLockService>();
        services.AddTransient<IReservationsService, ReservationsService>();
        services.Decorate<IReservationsService, ReservationsServiceWithTimeout>();
        services.AddTransient<ICommitmentV2Service, CommitmentsV2Service>();
        services.Decorate<ICommitmentV2Service, CommitmentsV2ServiceWithTimeout>();

        services.AddTransient<IHashingService>(_ => new HashingService.HashingService(configuration.AllowedHashstringCharacters, configuration.Hashstring));

    }
}
