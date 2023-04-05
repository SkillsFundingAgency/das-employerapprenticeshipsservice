using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IConfiguration _configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var idConfig = _configuration.GetSection("Identity")
            .Get<IdentityServerConfiguration>();
        var constants = new Constants(idConfig);

        services.AddControllersWithViews(ConfigureMvcOptions)
            .AddNewtonsoftJson(options =>
            {
                options.UseMemberCasing();
            });



        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        UserLinksViewModel.ChangePasswordLink = $"{constants.ChangePasswordLink()}{"https://" + _configuration.GetValue<string>("DashboardUrl") + "/service/password/change"}";
        UserLinksViewModel.ChangeEmailLink = $"{constants.ChangeEmailLink()}{"https://" + _configuration.GetValue<string>("DashboardUrl") + "/service/email/change"}";
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

    private void ConfigureMvcOptions(MvcOptions mvcOptions)
    { 
    }
}