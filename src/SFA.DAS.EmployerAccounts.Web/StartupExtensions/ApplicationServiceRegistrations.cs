using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Encoding;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, EmployerAccountsConfiguration configuration)
    {
        services.AddScoped<HtmlHelpers>();
        services.AddScoped<ActivitiesHelper>();
        services.AddTransient<IRestClientFactory, RestClientFactory>();
        services.AddTransient<IRestServiceFactory, RestServiceFactory>();
        services.AddTransient<IHttpServiceFactory, HttpServiceFactory>();
        services.AddTransient<IUserAornPayeLockService, UserAornPayeLockService>();

        services.AddTransient<IReservationsService, ReservationsService>();
        services.Decorate<IReservationsService, ReservationsServiceWithTimeout>();
        services.AddTransient<ICommitmentV2Service, CommitmentsV2Service>();
        services.Decorate<ICommitmentV2Service, CommitmentsV2ServiceWithTimeout>();

        services.AddTransient<IRecruitService, RecruitService>();
        services.Decorate<IRecruitService, RecruitServiceWithTimeout>();

        services.AddScoped<IAccountApiClient, AccountApiClient>();
        services.AddSingleton<IReferenceDataService, ReferenceDataService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IPensionRegulatorService, PensionRegulatorService>();

       
        services.AddTransient<IHashingService>(_ => new HashingService.HashingService(configuration.AllowedHashstringCharacters, configuration.Hashstring));

        services.AddTransient<IUserAccountRepository, UserAccountRepository>();

        services.AddScoped(typeof(ICookieService<>), typeof(HttpCookieService<>));
        services.AddScoped(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));
        services.AddScoped<IUrlActionHelper, UrlActionHelper>();

        services.AddScoped<IEncodingService, EncodingService>();

        return services;
    }
}
