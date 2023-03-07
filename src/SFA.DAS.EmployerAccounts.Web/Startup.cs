using FluentValidation;
using Microsoft.AspNetCore.Builder;
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
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Filters;
using SFA.DAS.EmployerAccounts.Web.Handlers;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private EmployerAccountsConfiguration _employerAccountsConfiguration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;

            _configuration = configuration.BuildDasConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_configuration);

            services.AddOptions();

            services.AddLogging();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddConfigurationOptions(_configuration);

            _employerAccountsConfiguration = _configuration.Get<EmployerAccountsConfiguration>();

            var identityServerConfiguration = _configuration
                .GetSection("Identity")
                .Get<IdentityServerConfiguration>();

            services.AddOrchestrators();
            services.AddAutoMapper(typeof(Startup).Assembly, typeof(AccountMappings).Assembly);
            services.AddAutoConfiguration();
            services.AddDatabaseRegistration(_employerAccountsConfiguration.DatabaseConnectionString);
            services.AddDataRepositories();
            services.AddApplicationServices(_employerAccountsConfiguration);
            services.AddHmrcServices();

            services.AddMaMenuConfiguration("SignOut", identityServerConfiguration.ClientId, _configuration["ResourceEnvironmentName"]);

            services.AddAuditServices(_employerAccountsConfiguration.AuditApi);
            services.AddCachesRegistrations();
            services.AddDateTimeServices(_configuration);
            services.AddEventsApi();
            services.AddNotifications(_configuration);

            services.AddEntityFrameworkUnitOfWork<EmployerAccountsDbContext>();
            services.AddNServiceBusClientUnitOfWork();
            services.AddEmployerAccountsApi();
            services.AddExecutionPolicies();
            services.AddEmployerAccountsOuterApi(_employerAccountsConfiguration.EmployerAccountsOuterApiConfiguration);
            services.AddCommittmentsV2Client(_employerAccountsConfiguration.CommitmentsApi);
            services.AddPollyPolicy(_employerAccountsConfiguration);
            services.AddContentApiClient(_employerAccountsConfiguration);
            services.AddProviderRegistration(_employerAccountsConfiguration);
            services.AddApprenticeshipLevyApi(_employerAccountsConfiguration);

            services.AddAuthenticationServices();

            services.AddMediatorCommandValidators();
            services.AddMediatorQueryValidators();
            services.AddMediatR(typeof(GetEmployerAccountByIdQuery));

            if (_configuration.UseGovUkSignIn())
            {
                services.AddAndConfigureGovUkAuthentication(_configuration,
                    $"{typeof(Startup).Assembly.GetName().Name}.Auth",
                    typeof(EmployerAccountPostAuthenticationClaimsHandler));
            }
            else
            {
                services.AddAndConfigureEmployerAuthentication(identityServerConfiguration);
            }

            // TODO: Support sign in 
            //services.AddAndConfigureSupportUserAuthentications(new SupportConsoleAuthenticationOptions());

            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

            services.Configure<RouteOptions>(options =>
            {

            }).AddMvc(options =>
            {
                options.Filters.Add(new AnalyticsFilter());
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
            serviceProvider.StartNServiceBus(_configuration, _configuration.IsDevOrLocal() || _configuration.IsTest());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseUnitOfWork

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        {
        }
    }
}
