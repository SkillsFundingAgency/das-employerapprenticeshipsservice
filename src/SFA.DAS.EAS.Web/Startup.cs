// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Routing;
using System.IdentityModel.Tokens;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.ViewModels;
using Karambolo.AspNetCore.Bundling.NUglify;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using SFA.DAS.OidcMiddleware;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;


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
        var constants = new Constants(_configuration.GetValue<IdentityServerConfiguration>("Identity"));
        var urlHelper = new UrlHelper(new ActionContext());

        services.AddControllersWithViews(ConfigureMvcOptions)
            .AddNewtonsoftJson(options =>
            {
                options.UseMemberCasing();
            });

        var idConfig = _configuration.GetSection("Identity")
            .Get<IdentityServerConfiguration>();


        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        UserLinksViewModel.ChangePasswordLink = $"{constants.ChangePasswordLink()}{urlHelper.Link("https://" + _configuration.GetValue<string>("DashboardUrl") + "/service/password/change", new { })}";
        UserLinksViewModel.ChangeEmailLink = $"{constants.ChangeEmailLink()}{urlHelper.Link("https://" + _configuration.GetValue<string>("DashboardUrl") + "/service/email/change", new { })}";
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseBundling(bundles => BundleConfig.RegisterBundles(bundles));

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

public class Constants
{
    private readonly string _baseUrl;
    private readonly IdentityServerConfiguration _configuration;

    public Constants(IdentityServerConfiguration configuration)
    {
        _baseUrl = configuration.ClaimIdentifierConfiguration.ClaimsBaseUrl;
        _configuration = configuration;
    }

    public string AuthorizeEndpoint() => $"{_configuration.BaseAddress}{_configuration.AuthorizeEndPoint}";
    public string ChangeEmailLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangeEmailLink, _configuration.ClientId);
    public string ChangePasswordLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangePasswordLink, _configuration.ClientId);
    public string DisplayName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.DisplayName;
    public string Email() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Email;
    public string FamilyName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.FaimlyName;
    public string GivenName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.GivenName;
    public string Id() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Id;
    public string LogoutEndpoint() => $"{_configuration.BaseAddress}{_configuration.LogoutEndpoint}";
    public string RegisterLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.RegisterLink, _configuration.ClientId);
    public string RequiresVerification() => _baseUrl + "requires_verification";
    public string TokenEndpoint() => $"{_configuration.BaseAddress}{_configuration.TokenEndpoint}";
    public string UserInfoEndpoint() => $"{_configuration.BaseAddress}{_configuration.UserInfoEndpoint}";
}
