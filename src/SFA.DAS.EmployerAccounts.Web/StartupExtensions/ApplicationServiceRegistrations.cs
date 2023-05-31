using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Policies.Hmrc;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.TasksApi;
using SFA.DAS.Encoding;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.UnitOfWork.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IAzureClientCredentialHelper, AzureClientCredentialHelper>();

        services.AddSingleton<IAccountEventFactory, AccountEventFactory>();
        services.AddSingleton<IPdfService, PdfService>();
        
        services.AddScoped<IHtmlHelpers, HtmlHelpers>();
        services.AddTransient<IHttpServiceFactory, HttpServiceFactory>();
        services.AddTransient<IUserAornPayeLockService, UserAornPayeLockService>();

        services.AddTransient<IReservationsService, ReservationsService>();
        services.Decorate<IReservationsService, ReservationsServiceWithTimeout>();

        services.AddTransient<ICommitmentV2Service, CommitmentsV2Service>();
        services.Decorate<ICommitmentV2Service, CommitmentsV2ServiceWithTimeout>();

        services.AddTransient<IRecruitService, RecruitService>();
        services.Decorate<IRecruitService, RecruitServiceWithTimeout>();

        services.AddScoped<IAccountApiClient, AccountApiClient>();

        services.AddHttpClient<ITaskApiClient, TaskApiClient>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IPensionRegulatorService, PensionRegulatorService>();

        services.AddTransient<IDateTimeService, DateTimeService>();

        services.AddSingleton<IEncodingService, EncodingService>();

        services.AddTransient<IUserAccountRepository, UserAccountRepository>();

        services.AddScoped(typeof(ICookieService<>), typeof(HttpCookieService<>));
        services.AddScoped(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));
        services.AddSingleton<IUrlActionHelper, UrlActionHelper>();

        services.AddTransient<HmrcExecutionPolicy>();

        services.AddTransient<IGenericEventFactory, GenericEventFactory>();
        services.AddTransient<IPayeSchemeEventFactory, PayeSchemeEventFactory>();
        services.AddTransient<ILegalEntityEventFactory, LegalEntityEventFactory>();
        services.AddTransient<IEmployerAgreementEventFactory, EmployerAgreementEventFactory>();
        services.AddScoped<IPayeSchemesWithEnglishFractionService, PayeSchemesWithEnglishFractionService>();

        services.AddTransient<IUserContext, UserContext>();

        services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();//TODO remove once gov login live
        
        return services;
    }
}
