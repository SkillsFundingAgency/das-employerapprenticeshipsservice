﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.StructureMap;

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
                .AddApiAuthentication();

            services.AddControllersWithViews(ConfigureMvcOptions)
                // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
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
