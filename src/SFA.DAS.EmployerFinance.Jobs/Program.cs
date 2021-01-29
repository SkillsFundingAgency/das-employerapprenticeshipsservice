using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Jobs.DependencyResolution;
using SFA.DAS.EmployerFinance.Startup;
using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;

namespace SFA.DAS.EmployerFinance.Jobs
{
    public class Program
    {
        public static void Main()
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            using (var container = IoC.Initialize())
            {
                var config = new JobHostConfiguration { JobActivator = new StructureMapJobActivator(container) };
                var loggerFactory = container.GetInstance<ILoggerFactory>();
                var startup = container.GetInstance<IStartup>();

                if (container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL))
                {
                    config.UseDevelopmentSettings();
                }

                config.LoggerFactory = loggerFactory;

                config.UseTimers();

                var jobHost = new JobHost(config);

                await startup.StartAsync();

                jobHost.RunAndBlock();

                await startup.StopAsync();
            }
        }
    }
}