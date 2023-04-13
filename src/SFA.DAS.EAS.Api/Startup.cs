using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using SFA.DAS.EAS.Account.Api.AuthPolicies;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.Encoding;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

        services.AddControllersWithViews()
           .AddNewtonsoftJson(opts => opts.UseMemberCasing());

        services.AddSingleton<IAuthorizationHandler, LoopBackHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("LoopBack", policy =>
            {
                policy.Requirements.Add(new LoopBackRequirement());
            }
            );
        });

        AutoMapper.ServiceCollectionExtensions.AddAutoMapper(services, typeof(Startup));
        services.AddSingleton(Configuration.GetSection("EmployerAccountsApi").Get<EmployerAccountsApiConfiguration>());
        services.AddSingleton(Configuration.GetSection("EmployerFinanceApi").Get<EmployerFinanceApiConfiguration>());
        services.AddSingleton<IEmployerAccountsApiHttpClientFactory, EmployerAccountsApiHttpClientFactory>();
        services.AddSingleton<IEmployerAccountsApiService, EmployerAccountsApiService>();
        services.AddSingleton<IEmployerFinanceApiHttpClientFactory, EmployerFinanceApiHttpClientFactory>();
        services.AddSingleton<IEmployerFinanceApiService, EmployerFinanceApiService>();
        services.AddScoped<StatisticsOrchestrator>();
        services.AddScoped<AccountsOrchestrator>();
        services.AddScoped<AccountTransactionsOrchestrator>();

        var hashstringChars = Configuration.GetValue<string>("AllowedHashstringCharacters");
        var hashstring = Configuration.GetValue<string>("Hashstring");

        services.AddSingleton<EncodingConfig>(new EncodingConfig
        {
            Encodings = new List<Encoding.Encoding>()
            {
                new Encoding.Encoding()
                {
                    EncodingType = "AccountId",
                    Salt = hashstring,
                    Alphabet = hashstringChars
                }
            }
        });

        services.AddSingleton<IEncodingService, EncodingService>();


        services.AddSwaggerGen();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
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
