using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Data;
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

        services.AddTransient<IRecruitService, RecruitService>();
        services.Decorate<IRecruitService, RecruitServiceWithTimeout>();
        
        services.AddScoped<IAccountApiClient, AccountApiClient>();
        services.AddSingleton<IReferenceDataService, ReferenceDataService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IPensionRegulatorService, PensionRegulatorService>();
        
        services.AddTransient<IApprenticeshipLevyApiClient>(s =>
        {
            var settings = s.GetService<IOptions<EmployerAccountsConfiguration>>().Value;
            var httpClient = new HttpClient();

            if (!settings.Hmrc.BaseUrl.EndsWith("/"))
            {
                settings.Hmrc.BaseUrl += "/";
            }
            httpClient.BaseAddress = new Uri(settings.Hmrc.BaseUrl);

            return new ApprenticeshipLevyApiClient(httpClient);
        });


        services.AddTransient<IHashingService>(_ => new HashingService.HashingService(configuration.AllowedHashstringCharacters, configuration.Hashstring));

        services.AddTransient<IUserAccountRepository, UserAccountRepository>();

    }
}
