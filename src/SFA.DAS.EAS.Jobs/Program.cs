using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Startup;
using SFA.DAS.EAS.Jobs.Startup;

namespace SFA.DAS.EAS.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                host.Run();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging() //todo: need to check logging/redis/use of localhost:6379 locally
                .UseApplicationInsights();  // todo: where does APPINSIGHTS_INSTRUMENTATIONKEY come from?

        //.UseDasEnvironment()
        //.UseStructureMap()
        //.UseConsoleLifetime()
        //.ConfigureServices(s => s.AddDasNServiceBus())
        //.ConfigureContainer<Registry>(IoC.Initialize);
    }
}
