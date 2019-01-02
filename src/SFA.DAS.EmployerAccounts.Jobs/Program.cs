using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;

namespace SFA.DAS.EmployerAccounts.Jobs
{
    public class Program
    {
        public static void Main()
        {
            Task.Run(MainAsync).GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            using (var container = IoC.Initialize())
            {
                var startup = container.GetInstance<EndpointStartup>();
                var config = new JobHostConfiguration { JobActivator = new StructureMapJobActivator(container) };
                var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);

                if (isDevelopment)
                {
                    config.UseDevelopmentSettings();
                }

                config.LoggerFactory = container.GetInstance<ILoggerFactory>();

                config.UseTimers();

                var jobHost = new JobHost(config);

                await startup.StartAsync();
                await jobHost.CallAsync(typeof(CreateReadStoreDatabaseJob).GetMethod(nameof(CreateReadStoreDatabaseJob.Run)));
                await jobHost.CallAsync(typeof(SeedAccountUsersJob).GetMethod(nameof(SeedAccountUsersJob.Run)));

                jobHost.RunAndBlock();

                await startup.StopAsync();
            }
        }
    }
}