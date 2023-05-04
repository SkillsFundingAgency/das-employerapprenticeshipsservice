using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.EAS.Account.Api.Authentication;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Account.Api.ServiceRegistrations;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Encoding;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.EAS.Account.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApiConfigurationSections(_configuration);

        services
            .AddApiAuthentication(_configuration, _configuration.IsDevOrLocal())
            .AddApiAuthorization();

        services.AddControllersWithViews()
                .AddNewtonsoftJson(opts => opts.UseMemberCasing());

        services.AddApplicationInsightsTelemetry();

        services.AddAutoMapper(typeof(Startup));

        services.AddSingleton<IEmployerAccountsApiHttpClientFactory, EmployerAccountsApiHttpClientFactory>();
        services.AddSingleton<IEmployerAccountsApiService, EmployerAccountsApiService>();
        services.AddSingleton<IEmployerFinanceApiHttpClientFactory, EmployerFinanceApiHttpClientFactory>();
        services.AddSingleton<IEmployerFinanceApiService, EmployerFinanceApiService>();

        services.AddOrchestrators();

        var hashstringChars = _configuration.GetValue<string>("AllowedHashstringCharacters");
        var hashstring = _configuration.GetValue<string>("Hashstring");

        services.AddSingleton(new EncodingConfig
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

        services.AddSwaggerGen(c =>
        {
            //c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "EAS API"
            });
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
        }).UseSwagger()
          .UseSwaggerUI(opt =>
          {
              opt.SwaggerEndpoint("/swagger/v1/swagger.json", "EAS API");
              opt.RoutePrefix = string.Empty;
          }); ;
    }
}
