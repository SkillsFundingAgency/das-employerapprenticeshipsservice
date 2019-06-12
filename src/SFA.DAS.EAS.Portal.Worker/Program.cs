using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.DependencyResolution;
using SFA.DAS.EAS.Portal.Worker.Startup;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.EAS.Portal.Worker.StartupJobs;

namespace SFA.DAS.EAS.Portal.Worker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();

                var jobHost = host.Services.GetService<IJobHost>();
                await jobHost.CallAsync(nameof(CreateReadStoreDatabaseJob.CreateReadStoreDatabase));

                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging()
                .UseApplicationInsights()
                .UseDasEnvironment()
                .UseConsoleLifetime()
                .ConfigureServices((c, s) => s.AddApplicationServices(c))
                .ConfigureServices((c, s) => s.AddHashingServices(c))
                .ConfigureServices((c, s) => s.AddCommitmentsApiConfiguration(c))
                .ConfigureServices((c, s) => s.AddProviderServices(c))
                .ConfigureServices((c, s) => s.AddCosmosDatabase(c))
                .ConfigureServices((c, s) => s.AddDasNServiceBus());
    }
}