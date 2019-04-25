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
        //https://github.com/Azure/azure-webjobs-sdk/wiki/Application-Insights-Integration
        //https://github.com/Azure/azure-webjobs-sdk/blob/dev/sample/SampleHost/Program.cs
        //https://docs.microsoft.com/en-us/azure/app-service/webjobs-sdk-how-to

        //todo: functions instead? https://github.com/tmasternak/NServiceBus.Functions
        //todo: not finding helloworld webjob
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging() //todo: need to check logging/redis/use of localhost:6379 locally
                .UseApplicationInsights() // todo: need to add APPINSIGHTS_INSTRUMENTATIONKEY to config somewhere. where does it normally live? we could store it in table storage
                .UseDasEnvironment();

        //.UseStructureMap()
        //.ConfigureServices(s => s.AddDasNServiceBus())
        //.ConfigureContainer<Registry>(IoC.Initialize);
    }
}