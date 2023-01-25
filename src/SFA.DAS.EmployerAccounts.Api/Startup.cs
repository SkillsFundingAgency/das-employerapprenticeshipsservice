using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.EmployerAccounts.Api.Authentication;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.Filters;
using SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.EmployerAccounts.Api;

public class Startup
{
    public IConfiguration Configuration { get; }
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _environment = environment;
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var employerAccountsConfiguration = Configuration.Get<EmployerAccountsConfiguration>();

        services.AddApiConfigurationSections(Configuration)
            .AddApiAuthentication(Configuration)
            .AddApiAuthorization(_environment)
            .Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;

            })
            .AddMvc(opt =>
            {
                opt.AddAuthorization();
                opt.Filters.Add<ValidateModelStateFilter>();
                opt.Filters.Add<StopwatchFilter>();
            });

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Employer Accounts API"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        services.AddDasDistributedMemoryCache(employerAccountsConfiguration, _environment.IsDevelopment());
        services.AddDasHealthChecks(employerAccountsConfiguration);
        services.StartNServiceBus(employerAccountsConfiguration, Configuration.IsDevOrLocal());
        services.AddDatabaseRegistration(employerAccountsConfiguration, Configuration["EnvironmentName"]);
        services.AddDataRepositories(); 
        services.AddEventsApi();
        services.AddExecutionPolicies();
        services.AddHashingServices(employerAccountsConfiguration);
        services.AddAutoMapper(typeof(Startup).Assembly);
        services.AddMediatR(typeof(Startup).Assembly);
        services.AddNotifications(Configuration);

        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("text/html"));
        });

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
        }

        app.UseHttpsRedirection()
            .UseSwagger()
            .UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Employer Accounts API");
                opt.RoutePrefix = string.Empty;
            })
            .UseStaticFiles()
            .UseDasHealthChecks()
            .UseUnauthorizedAccessExceptionHandler()
            .UseAuthentication()
            .UseUnitOfWork()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

    }
}