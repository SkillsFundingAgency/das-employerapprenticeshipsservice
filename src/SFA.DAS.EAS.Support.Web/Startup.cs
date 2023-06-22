using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.Web.Configuration;
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
        services.AddLogging();
        
        services.AddConfigurationSections(_configuration);

        services.AddSingleton(_configuration);
        var webConfiguration = _configuration.Get<EasSupportConfiguration>();

        services.AddAndConfigureSupportAuthentication(webConfiguration);

        services.AddApiClientServices();
        services.AddRepositories();
        services.AddApplicationServices();

        services.AddApplicationInsightsTelemetry();

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
            endpoints.MapDefaultControllerRoute();
        });
    }
}
