using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.Startup;

namespace SFA.DAS.EmployerAccounts.MessageHandlers
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
                var config = new JobHostConfiguration();
                var startup = container.GetInstance<IStartup>();

                if (container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL))
                {
                    config.UseDevelopmentSettings();
                }

                var jobHost = new JobHost(config);

                await startup.StartAsync();
                await jobHost.CallAsync(typeof(Program).GetMethod(nameof(Block)));

                jobHost.RunAndBlock();

                await startup.StopAsync();
            }
        }

        [NoAutomaticTrigger]
        public static async Task Block(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(3000, cancellationToken);
            }
        }
    }
}
