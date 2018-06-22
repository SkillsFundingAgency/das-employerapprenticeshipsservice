using Microsoft.Azure.WebJobs;
using NServiceBus;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.EAS.Jobs.DependencyResolution;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Jobs
{
    public class Program
    {
        private static IEndpointInstance _endpointInstance;

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

            _endpointInstance?.Stop().GetAwaiter().GetResult();
        }

        [NoAutomaticTrigger]
        public static async Task AsyncMain(CancellationToken cancellationToken, bool isDevelopment)
        {
            var container = IoC.Initialize();

            ServiceLocator.Initialize(container);

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Jobs");
            endpointConfiguration.Setup(container, isDevelopment);

            _endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }
    }
}
