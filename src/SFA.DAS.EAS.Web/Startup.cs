using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Application.ServiceRegistrations;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.RouteValues;
using SFA.DAS.EAS.Web.StartupExtensions;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.EAS.Web;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, bool buildConfig = true)
    {
        _configuration = buildConfig ? configuration.BuildDasConfiguration() : configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);

        var identityServerConfiguration = _configuration
            .GetSection("Identity")
            .Get<IdentityServerConfiguration>();

        services.AddHttpContextAccessor();
        services.AddConfigurationSections(_configuration);

        var easConfiguration = _configuration.Get<EmployerApprenticeshipsServiceConfiguration>();

        if (easConfiguration.UseGovSignIn)
        {
            services.AddMaMenuConfiguration(RouteNames.SignOut, _configuration["ResourceEnvironmentName"]);
        }
        else
        {
            services.AddMaMenuConfiguration(RouteNames.SignOut, identityServerConfiguration.ClientId, _configuration["ResourceEnvironmentName"]);
        }

        services.AddOuterApiClient(easConfiguration.EmployerAccountsOuterApiConfiguration);
        services.AddAuthenticationServices();

        var govConfig = _configuration.GetSection("SFA.DAS.Employer.GovSignIn");
        govConfig["ResourceEnvironmentName"] = _configuration["ResourceEnvironmentName"];
        govConfig["StubAuth"] = _configuration["StubAuth"];
            
        services.AddAndConfigureGovUkAuthentication(govConfig, new AuthRedirects
        {
            SignedOutRedirectUrl = "",
            LocalStubLoginPath = "/service/SignIn-Stub",
        }, null, typeof(UserAccountService));

        services.AddControllersWithViews()
            .AddNewtonsoftJson(options => { options.UseMemberCasing(); });

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        SetupUserLinks(identityServerConfiguration);

        services.AddApplicationInsightsTelemetry();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
    }

    private void SetupUserLinks(IdentityServerConfiguration identityServerConfiguration)
    {
        var constants = new Constants(identityServerConfiguration);

        UserLinksViewModel.ChangePasswordLink = $"{constants.ChangePasswordLink()}{$"https://{_configuration.GetValue<string>("DashboardUrl")}/service/password/change"}";
        UserLinksViewModel.ChangeEmailLink = $"{constants.ChangeEmailLink()}{$"https://{_configuration.GetValue<string>("DashboardUrl")}/service/email/change"}";
    }
}