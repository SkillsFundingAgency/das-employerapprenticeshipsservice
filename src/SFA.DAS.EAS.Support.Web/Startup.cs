using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Support.Web.Extensions;
using SFA.DAS.EAS.Support.Web.ServiceRegistrations;

namespace SFA.DAS.EAS.Support.Web;

public class Startup
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();

        services.AddConfigurationSections(_configuration);

        services.AddSingleton(_configuration);

        services.AddApiClientServices();
        services.AddRepositories();
        services.AddApplicationServices();

        var supportConfiguration = _configuration.Get<EasSupportConfiguration>();
        services.AddActiveDirectoryAuthentication(supportConfiguration);

        services.AddControllersWithViews(opt =>
        {
            if (!_environment.IsDevelopment())
            {
                opt.Filters.Add(new AuthorizeFilter(PolicyNames.Default));
            }
        }).AddNewtonsoftJson(options => { options.UseMemberCasing(); });

        services.AddApplicationInsightsTelemetry();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
            app.UseAuthentication();
        }
        
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}