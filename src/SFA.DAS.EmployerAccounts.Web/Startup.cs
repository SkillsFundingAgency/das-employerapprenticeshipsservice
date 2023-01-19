using System.IO;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Authorization.EmployerFeatures.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Filters;
using SFA.DAS.EmployerAccounts.Web.Handlers;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;
using SFA.DAS.GovUK.Auth.AppStart;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

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
            if (!configuration.IsDev())
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["Environment"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }

            _configuration = config.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();

            var employerAccountsConfiguration = _configuration
                .GetSection(nameof(EmployerAccountsConfiguration))
                .Get<EmployerAccountsConfiguration>();

            var identityServerConfiguration = _configuration
                .GetSection(nameof(IdentityServerConfiguration))
                .Get<IdentityServerConfiguration>();

            services.AddConfigurationOptions(_configuration, employerAccountsConfiguration);
            services.AddOrchestrators();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddDatabaseRegistration(employerAccountsConfiguration, _configuration["Environment"]);
            services.AddApplicationServices(employerAccountsConfiguration);
            services.AddHashingServices(employerAccountsConfiguration);
            services.AddAuditServices();
            services.AddCachesRegistrations();
            services.AddDateTimeServices(_configuration);
            services.AddEventsApi();
            services.AddNotifications(_configuration);

            services.AddEmployerFeaturesAuthorization();
            services.AddDasAuthorization();
            services.AddEmployerAccountsApi();
            services.AddExecutionPolicies();
            services.AddActivitiesClient(_configuration);
            services.AddEmployerAccountsOuterApi(employerAccountsConfiguration, _configuration);
            services.AddCommittmentsV2Client();
            services.AddPollyPolicy(employerAccountsConfiguration);
            services.AddContentApiClient(employerAccountsConfiguration, _configuration);
            services.AddProviderRegistration(employerAccountsConfiguration, _configuration);
            services.AddApprenticeshipLevyApi(employerAccountsConfiguration);

            services.AddAuthenticationServices();

            services.AddMediatR(typeof(Startup).Assembly);

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

            services.AddLogging();
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

            services.AddAdvancedDependencyInjection();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseAdvancedDependencyInjection();
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        {
        }
    }
}
