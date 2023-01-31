using System.IO;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Authorization.EmployerFeatures.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.EmployerAccounts.Web.Filters;
using SFA.DAS.EmployerAccounts.Web.Handlers;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.Microsoft;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

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

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
            if (!configuration.IsDev())
            {
                config.AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif

            config.AddEnvironmentVariables();

            if (!configuration.IsTest())
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }

            _configuration = config.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddOptions();

            services.AddLogging();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddConfigurationOptions(_configuration);

            _employerAccountsConfiguration = _configuration.Get<EmployerAccountsConfiguration>();

            var identityServerConfiguration = _configuration
                .GetSection("Identity")
                .Get<IdentityServerConfiguration>();

            services.AddOrchestrators();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddDatabaseRegistration(_employerAccountsConfiguration, _configuration["Environment"]);
            services.AddDataRepositories();
            services.AddApplicationServices(_employerAccountsConfiguration);
            services.AddHashingServices(_employerAccountsConfiguration);
            services.AddAuditServices();
            services.AddCachesRegistrations();
            services.AddDateTimeServices(_configuration);
            services.AddEventsApi();
            services.AddNotifications(_configuration);

            services.AddNServiceBusUnitOfWork();
            services.StartNServiceBus(_employerAccountsConfiguration, _configuration.IsDevOrLocal());
            services.AddEmployerFeaturesAuthorization();
            services.AddDasAuthorization();
            services.AddEmployerAccountsApi();
            services.AddExecutionPolicies();
            services.AddEmployerAccountsOuterApi(_employerAccountsConfiguration.EmployerAccountsOuterApiConfiguration, _configuration);
            services.AddCommittmentsV2Client();
            services.AddPollyPolicy(_employerAccountsConfiguration);
            services.AddContentApiClient(_employerAccountsConfiguration, _configuration);
            services.AddProviderRegistration(_employerAccountsConfiguration, _configuration);
            services.AddApprenticeshipLevyApi(_employerAccountsConfiguration);

            services.AddAuthenticationServices();

            services.AddMediatorCommandValidators();
            services.AddMediatorQueryValidators();
            services.AddMediatR(typeof(GetEmployerAccountByHashedIdQuery));

            if (_configuration["EmployerAccountsConfiguration:UseGovSignIn"] != null &&
                _configuration["EmployerAccountsConfiguration:UseGovSignIn"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddAndConfigureGovUkAuthentication(_configuration,
                    $"{typeof(Startup).Assembly.GetName().Name}.Auth",
                    typeof(EmployerAccountPostAuthenticationClaimsHandler));
            }
            else
            {
                services.AddAndConfigureEmployerAuthentication(identityServerConfiguration);
            }

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
                o.AddAuthorization();
            }).AddRazorRuntimeCompilation();
#endif

            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseUnauthorizedAccessExceptionHandler();

            app.UseStaticFiles();
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
