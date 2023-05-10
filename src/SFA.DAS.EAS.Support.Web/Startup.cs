using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            {
                options.UseMemberCasing();
            });

        services.AddConfigurationSections(_configuration);
        services.AddApiClientServices();
        services.AddRepositories();
        services.AddApplicationServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

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
}
