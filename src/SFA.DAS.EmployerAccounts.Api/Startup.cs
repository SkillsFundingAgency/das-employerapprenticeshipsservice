using System.Collections.Generic;
using System.IO;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Api.Authentication;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.ErrorHandler;
using SFA.DAS.EmployerAccounts.Api.Filters;
using SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Authorisation;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;
using SFA.DAS.Validation.Mvc.Extensions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.EmployerAccounts.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _environment = environment;

        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
        if (!configuration.IsDev())
        {
            config.AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif

        config.AddEnvironmentVariables();

        if (!configuration.IsTest())
        {
            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                    options.ConfigurationKeysRawJsonResult = new[] { "SFA.DAS.Encoding" };
                }
            );
        }
        _configuration = config.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var employerAccountsConfiguration = _configuration.Get<EmployerAccountsConfiguration>();
        var isDevelopment = _configuration.IsDevOrLocal();

        services
            .AddApiAuthentication(_configuration, isDevelopment)
            .AddApiAuthorization(isDevelopment);

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Employer Accounts API"
            });
        });

        services.AddApplicationServices();
        services.AddDasDistributedMemoryCache(employerAccountsConfiguration, _environment.IsDevelopment());
        services.AddDasHealthChecks(employerAccountsConfiguration);
        services.AddOrchestrators();

        services.AddEntityFrameworkUnitOfWork<EmployerAccountsDbContext>();
        services.AddNServiceBusClientUnitOfWork();

        services.AddDatabaseRegistration(employerAccountsConfiguration.DatabaseConnectionString);
        services.AddDataRepositories();
        //services.AddEventsApi(employerAccountsConfiguration);
        services.AddExecutionPolicies();

        services.AddAutoMapper(typeof(AccountMappings), typeof(Startup));

        services.AddMediatorValidators();
        services.AddMediatR(typeof(GetPayeSchemeByRefQuery));
        services.AddNotifications(_configuration);

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IAuthenticationServiceWrapper, AuthenticationServiceWrapper>();

        services.AddApiConfigurationSections(_configuration)
            .Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; })
            .AddMvc(opt =>
            {
                if (!_configuration.IsDevOrLocal() && !_configuration.IsTest())
                {
                    opt.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }

                opt.AddValidation();

                opt.Filters.Add<StopwatchFilter>();
            });

        services.AddApplicationInsightsTelemetry();
    }

    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        serviceProvider.StartNServiceBus(_configuration, _configuration.IsDevOrLocal() || _configuration.IsTest());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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

        app.UseHttpsRedirection()
            .UseApiGlobalExceptionHandler(loggerFactory.CreateLogger("Startup"))
            .UseStaticFiles()
            .UseDasHealthChecks()
            .UseUnitOfWork()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            })
          .UseSwagger()
          .UseSwaggerUI(opt =>
          {
              opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Employer Accounts API");
              opt.RoutePrefix = string.Empty;
          });
    }
}