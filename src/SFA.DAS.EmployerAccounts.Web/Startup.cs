using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.EmployerAccounts.Startup;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Filters;
using SFA.DAS.EmployerAccounts.Web.Handlers;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;

namespace SFA.DAS.EmployerAccounts.Web;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment, bool buildConfig = true)
    {
        _environment = environment;

        _configuration = buildConfig ? configuration.BuildDasConfiguration() : configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);

        services.AddOptions();

        services.AddLogging();

        services.AddHttpContextAccessor();

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddConfigurationOptions(_configuration);
        var identityServerConfiguration = _configuration
            .GetSection(ConfigurationKeys.Identity)
            .Get<IdentityServerConfiguration>();

        var employerAccountsConfiguration = _configuration.GetSection(ConfigurationKeys.EmployerAccounts).Get<EmployerAccountsConfiguration>();

        services.AddOrchestrators();
        services.AddAutoMapper(typeof(Startup).Assembly, typeof(AccountMappings).Assembly);
        services.AddAutoConfiguration();
        services.AddDatabaseRegistration();
        services.AddDataRepositories();
        services.AddApplicationServices();
        services.AddHmrcServices();

        if (employerAccountsConfiguration.UseGovSignIn)
        {
            services.AddMaMenuConfiguration(RouteNames.SignOut, _configuration["ResourceEnvironmentName"]);
        }
        else
        {
            services.AddMaMenuConfiguration(RouteNames.SignOut, identityServerConfiguration.ClientId, _configuration["ResourceEnvironmentName"]);
        }

        services.AddAuditServices();
        services.AddCachesRegistrations();
        services.AddDateTimeServices(_configuration);
        services.AddEventsApi();
        services.AddNotifications(_configuration);

        services
            .AddUnitOfWork()
            .AddEntityFramework(employerAccountsConfiguration)
            .AddEntityFrameworkUnitOfWork<EmployerAccountsDbContext>();
        services.AddNServiceBusClientUnitOfWork();
        services.AddEmployerAccountsApi();
        services.AddExecutionPolicies();
        services.AddEmployerAccountsOuterApi(employerAccountsConfiguration.EmployerAccountsOuterApiConfiguration);
        services.AddCommittmentsV2Client(employerAccountsConfiguration.CommitmentsApi);
        services.AddPollyPolicy(employerAccountsConfiguration);
        services.AddContentApiClient(employerAccountsConfiguration);
        services.AddProviderRegistration(employerAccountsConfiguration);
        services.AddApprenticeshipLevyApi(employerAccountsConfiguration);
        services.AddReferenceDataApi();

        services.AddAuthenticationServices();

        services.AddMediatorValidators();
        services.AddMediatR(typeof(GetEmployerAccountByIdQuery));

        var authenticationBuilder = services.AddAuthentication();

        if (_configuration.UseGovUkSignIn())
        {
            var govConfig = _configuration.GetSection("SFA.DAS.Employer.GovSignIn");
            govConfig["ResourceEnvironmentName"] = _configuration["ResourceEnvironmentName"];
            govConfig["StubAuth"] = _configuration["StubAuth"];
            services.AddAndConfigureGovUkAuthentication(govConfig,
                typeof(EmployerAccountPostAuthenticationClaimsHandler),
                "",
                "/service/SignIn-Stub");
        }
        else
        {
            services.AddAndConfigureEmployerAuthentication(identityServerConfiguration);
        }

        var staffAuthConfig = new SupportConsoleAuthenticationOptions
        {
            AdfsOptions = new ADFSOptions
            {
                MetadataAddress = employerAccountsConfiguration.AdfsMetadata,
                Wreply = employerAccountsConfiguration.EmployerAccountsBaseUrl,
                Wtrealm = employerAccountsConfiguration.EmployerAccountsBaseUrl,
                BaseUrl = employerAccountsConfiguration.Identity.BaseAddress,
            }
        };

        authenticationBuilder.AddAndConfigureSupportConsoleAuthentication(staffAuthConfig);

        services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

        services.Configure<RouteOptions>(options =>
        {

        }).AddMvc(options =>
        {
            options.Filters.Add(new AnalyticsFilterAttribute());
            if (!_configuration.IsDev())
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }

        });

        services.AddApplicationInsightsTelemetry();

        if (!_environment.IsDevelopment())
        {
            services.AddHealthChecks();
            services.AddDataProtection(_configuration);
        }

#if DEBUG
        services.AddControllersWithViews(o =>
        {
        }).AddRazorRuntimeCompilation();
#endif

        services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
    }

    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        serviceProvider.StartNServiceBus(_configuration.IsDevOrLocal(), ServiceBusEndpointType.Web);

        // Replacing ClientOutboxPersisterV2 with a local version to fix unit of work issue due to propogating Task up the chain rather than awaiting on DB Command.
        // not clear why this fixes the issue. Attempted to make the change in SFA.DAS.Nservicebus.SqlServer however it conflicts when upgraded with SFA.DAS.UnitOfWork.Nservicebus
        // which would require upgrading to NET6 to resolve.
        var serviceDescriptor = serviceProvider.FirstOrDefault(serv => serv.ServiceType == typeof(IClientOutboxStorageV2));
        serviceProvider.Remove(serviceDescriptor);
        serviceProvider.AddScoped<IClientOutboxStorageV2, AppStart.ClientOutboxPersisterV2>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSupportConsoleAuthentication();

        app.UseUnitOfWork();

        app.UseStaticFiles();

        app.UseMiddleware<RobotsTextMiddleware>();

        app.UseAuthentication();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            Secure = CookieSecurePolicy.Always,
            MinimumSameSitePolicy = SameSiteMode.None,
            HttpOnly = HttpOnlyPolicy.Always
        });

        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}