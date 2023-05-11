using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Application.ServiceRegistrations;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Support.Web.Extensions;
using SFA.DAS.EAS.Support.Web.ServiceRegistrations;

namespace SFA.DAS.EAS.Support.Web;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var easConfiguration = _configuration.Get<EmployerApprenticeshipsServiceConfiguration>();
        var identityServerConfiguration = _configuration
               .GetSection("Identity")
               .Get<IdentityServerConfiguration>();

        services.AddOuterApi(easConfiguration.EmployerAccountsOuterApiConfiguration);
        services.AddAuthenticationServices();
        services.AddAndConfigureEmployerAuthentication(identityServerConfiguration);

        services.AddConfigurationSections(_configuration);
        services.AddApiClientServices();
        services.AddRepositories();
        services.AddApplicationServices();

        services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            {
                options.UseMemberCasing();
            });
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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
