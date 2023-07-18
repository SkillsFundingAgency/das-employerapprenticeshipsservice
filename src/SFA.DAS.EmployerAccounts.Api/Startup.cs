using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using SFA.DAS.EmployerAccounts.Api.Authentication;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.ErrorHandler;
using SFA.DAS.EmployerAccounts.Api.Extensions;
using SFA.DAS.EmployerAccounts.Api.Filters;
using SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Authorisation;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.EmployerAccounts.Startup;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.EmployerAccounts.Api;

[ExcludeFromCodeCoverage]
public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration.BuildDasConfiguration();
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

        services
            .AddUnitOfWork()
            .AddEntityFramework(employerAccountsConfiguration)
            .AddEntityFrameworkUnitOfWork<EmployerAccountsDbContext>();

        services.AddNServiceBusClientUnitOfWork();

        services.AddDatabaseRegistration();
        services.AddDataRepositories();
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
                if (!_configuration.IsDevOrLocal())
                {
                    opt.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }

                opt.AddValidation();

                opt.Filters.Add<StopwatchFilterAttribute>();
            });

        services.AddApplicationInsightsTelemetry();
    }

    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        serviceProvider.StartNServiceBus(_configuration.IsDevOrLocal(), ServiceBusEndpointType.Api);
        // Replacing ClientOutboxPersisterV2 with a local version to fix unit of work issue due to propogating Task up the chain rather than awaiting on DB Command.
        // not clear why this fixes the issue. Attempted to make the change in SFA.DAS.Nservicebus.SqlServer however it conflicts when upgraded with SFA.DAS.UnitOfWork.Nservicebus
        // which would require upgrading to NET6 to resolve.
        var serviceDescriptor = serviceProvider.FirstOrDefault(serv => serv.ServiceType == typeof(IClientOutboxStorageV2));
        serviceProvider.Remove(serviceDescriptor);
        serviceProvider.AddScoped<IClientOutboxStorageV2, AppStart.ClientOutboxPersisterV2>();
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
            .UseAuthentication()
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