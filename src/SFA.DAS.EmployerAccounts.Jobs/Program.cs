using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs
{
    public class Program
    {
        public static async Task Main()
        {
            var container = IoC.Initialize();

            using (var host = CreateHost(container))
            {
                var jobHost = host.Services.GetService(typeof(IJobHost)) as JobHost;

                await host.StartAsync();

                await jobHost.CallAsync(typeof(CreateReadStoreDatabaseJob).GetMethod(nameof(CreateReadStoreDatabaseJob.Run)));
                await jobHost.CallAsync(typeof(SeedAccountUsersJob).GetMethod(nameof(SeedAccountUsersJob.Run)));

                await host.RunAsync();

                await host.StopAsync();
            }
        }

        private static IHost CreateHost(IContainer container)
        {
            var builder = new HostBuilder()
                 .ConfigureWebJobs(config =>
                 {
                     config.AddTimers();
                 })
                 .ConfigureLogging((context, loggingBuilder) =>
                 {
                     var appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                     if (!string.IsNullOrEmpty(appInsightsKey))
                     {
                         loggingBuilder.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = appInsightsKey);
                     }
                 }).ConfigureServices((context, services) =>
                 {
                     services.AddScoped<IJobActivator, StructureMapJobActivator>();
                 });

            var isDevelopment = container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL);

            if (isDevelopment)
            {
                builder.UseEnvironment("development");
            }

            return builder.Build();
        }
    }
}