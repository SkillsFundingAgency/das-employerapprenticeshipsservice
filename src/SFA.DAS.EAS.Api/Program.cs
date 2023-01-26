using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

using SFA.DAS.EAS.Account.Api.AuthPolicies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(opts => opts.UseMemberCasing());

builder.Services.AddSingleton<IAuthorizationHandler, LoopBackHandler>();
builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("LoopBack", policy => policy.Requirements.Add(new LoopBackRequirement()));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
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

app.Run();
