using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution;
using SFA.DAS.EAS.Portal.Client.TestHarness.Scenarios;
using SFA.DAS.EAS.Portal.Client.TestHarness.Startup;
using SFA.DAS.EAS.Portal.Startup;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness
{
    //todo: logging?
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();

                var getAccount = host.Services.GetService<GetAccountScenario>();
                await getAccount.Run();

                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                //todo: structuremap di
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging()
                .UseDasEnvironment()
                .UseStructureMap()
                .ConfigureServices(s => s.AddTransient<GetAccountScenario, GetAccountScenario>())
                .ConfigureContainer<Registry>(IoC.Initialize);
    }
}