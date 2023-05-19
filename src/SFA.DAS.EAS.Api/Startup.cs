using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.EAS.Account.Api.Authentication;
using SFA.DAS.EAS.Account.Api.Extensions;
using SFA.DAS.EAS.Account.Api.Filters;
using SFA.DAS.EAS.Account.Api.ServiceRegistrations;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Encoding;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.EAS.Account.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApiConfigurationSections(_configuration);

        services
            .AddApiAuthentication(_configuration, _configuration.IsDevOrLocal());
            //.AddApiAuthorization();


        services.AddAutoMapper(typeof(Startup));
        services.AddClientServices();
        services.AddOrchestrators();

        services.AddSingleton<IEncodingService, EncodingService>();

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "EAS API"
            });
        });

        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; })
            .AddMvc(opt =>
            {
                if (!_configuration.IsDevOrLocal())
                {
                    opt.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }

                opt.AddValidation();

                opt.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            });

        services.AddApplicationInsightsTelemetry();
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
            endpoints.MapControllers();
        })
        .UseSwagger()
        .UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Employer Finance API");
            opt.RoutePrefix = "swagger";
        });
    }
}
