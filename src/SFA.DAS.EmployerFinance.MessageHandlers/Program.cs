using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerFinance.Startup;

namespace SFA.DAS.EmployerFinance.MessageHandlers
{
    public class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            using (var container = IoC.Initialize())
            {
                var config = new JobHostConfiguration();
                var startup = container.GetInstance<IStartup>();
                var configtest = container.GetInstance<EmployerFinanceConfiguration>();

                if (ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local))
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
