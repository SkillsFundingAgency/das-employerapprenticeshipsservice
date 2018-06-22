using Microsoft.Azure.WebJobs;
using NServiceBus;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.EAS.MessageHandlers.DependencyResolution;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers
{
    public class Program
    {
        public static void Main()
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);
            var config = new JobHostConfiguration();

            if (isDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            host.Call(typeof(Program).GetMethod(nameof(Program.AsyncMain)), new { isDevelopment });
            host.RunAndBlock();
        }

        [NoAutomaticTrigger]
        public static async Task AsyncMain(CancellationToken cancellationToken, bool isDevelopment)
        {
            var container = IoC.Initialize();
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.MessageHandlers");

            endpointConfiguration.Setup(container, isDevelopment);

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(3000, cancellationToken)
                    .ConfigureAwait(false);
            }

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
