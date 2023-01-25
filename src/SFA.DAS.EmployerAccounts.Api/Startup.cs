using System.Reflection;
using System;
using System.IO;
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

namespace SFA.DAS.EmployerAccounts.Api
{
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
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Employer Accounts API"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //c.AddRegistry<AuthorizationRegistry>();
            //c.AddRegistry<CachesRegistry>();
            //c.AddRegistry<ConfigurationRegistry>();
            //c.AddRegistry<DataRegistry>();
            //// c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<EmployerAccountsDbContext>>();
            //c.AddRegistry<EventsRegistry>();
            //c.AddRegistry<ExecutionPoliciesRegistry>();
            //c.AddRegistry<HashingRegistry>();
            //c.AddRegistry<LoggerRegistry>();
            //c.AddRegistry<MapperRegistry>();
            //c.AddRegistry<MediatorRegistry>();
            //c.AddRegistry<MessagePublisherRegistry>();
            //c.AddRegistry<NotificationsRegistry>();
            //c.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
            //c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
            //c.AddRegistry<RepositoriesRegistry>();
            //c.AddRegistry<DefaultRegistry>();

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
                });

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
}
