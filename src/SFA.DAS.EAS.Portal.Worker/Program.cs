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
                //todo: separate out these, passing configuration???
                .ConfigureServices(s => s.AddApplicationServices())
                .ConfigureServices(s => s.AddProviderServices())
                .ConfigureServices(s => s.AddCosmosDatabase())
                .ConfigureServices(s => s.AddDasNServiceBus());
    }
}