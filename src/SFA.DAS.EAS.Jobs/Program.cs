using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Startup;
using SFA.DAS.EAS.Jobs.Startup;

namespace SFA.DAS.EAS.Jobs
{
    class Program
    {
        //useful links
        //https://stackoverflow.com/questions/51970969/how-to-use-hostbuilder-for-webjob

        //todo: functions instead? https://github.com/tmasternak/NServiceBus.Functions
        static async Task Main(string[] args)
        {
            //todo: instead of building, running and UseConsoleLifetime, can we instead...
            await CreateHostBuilder(args).RunConsoleAsync();
            //using (var host = CreateHostBuilder(args).Build())
            //{
            //    host.Run();
            //}
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging() //todo: need to check logging/redis/use of localhost:6379 locally
                //todo: blows up as no options: use overload with options rather than just passing key
                //.UseApplicationInsights() // todo: where does APPINSIGHTS_INSTRUMENTATIONKEY come from?
                .UseDasEnvironment();
        //.UseConsoleLifetime();


        //.UseStructureMap()
        //.ConfigureServices(s => s.AddDasNServiceBus())
        //.ConfigureContainer<Registry>(IoC.Initialize);
    }
}