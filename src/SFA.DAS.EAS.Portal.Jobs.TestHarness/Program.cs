using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Jobs.TestHarness.Scenarios;
using SFA.DAS.EAS.Portal.Jobs.TestHarness.Startup;
using SFA.DAS.EAS.Portal.Startup;

namespace SFA.DAS.EAS.Portal.Jobs.TestHarness
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();

                var publishReserveFundingAddedEvent = host.Services.GetService<PublishReserveFundingAddedEvent>();
                await publishReserveFundingAddedEvent.Run();

                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging()
                .UseDasEnvironment()
                //.UseStructureMap()
                .ConfigureServices(s => s.AddDasNServiceBus())
                .ConfigureServices(s => s.AddTransient<PublishReserveFundingAddedEvent, PublishReserveFundingAddedEvent>());
        //todo: DI

        //.ConfigureContainer<Registry>(IoC.Initialize);
    }
}