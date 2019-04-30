using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Jobs.Startup;
using SFA.DAS.EAS.Portal.Jobs.StartupJobs;
using SFA.DAS.EAS.Portal.Startup;

namespace SFA.DAS.EAS.Portal.Jobs
{
    class Program
    {
        // to run
        //   local
        //     set environment variables
        //       APPSETTING_EnvironmentName                        LOCAL (currently set in launchsettings.json)
        //       APPSETTING_ConfigurationStorageConnectionString   storage account containing config tables (can be emulator)
        //       AzureWebJobsStorage                               \ real storage account (not emulator)
        //       AzureWebJobsDashboard                             /
        //       todo: ^ ConnectionStrings: prefix??
        //   real published app service
        //       Application Settings
        //         EnvironmentName                                LOCAL
        //         ConfigurationStorageConnectionString           storage account containing config tables
        //       Connection Strings
        //         AzureWebJobsStorage                            \ storage account
        //         AzureWebJobsDashboard                          /

        //useful links
        //https://stackoverflow.com/questions/51970969/how-to-use-hostbuilder-for-webjob
        //https://github.com/Azure/azure-webjobs-sdk/wiki/Application-Insights-Integration
        //https://github.com/Azure/azure-webjobs-sdk/blob/dev/sample/SampleHost/Program.cs
        //https://docs.microsoft.com/en-us/azure/app-service/webjobs-sdk-how-to

        //todo: how to see & trigger individual jobs in v3?
        //https://github.com/Azure/azure-webjobs-sdk/issues/1975

        //todo: test app insights

        //todo: rename if can combine jobs/message handler

        //todo: functions instead? https://github.com/tmasternak/NServiceBus.Functions

        //todo: devops
        // ServiceBusConnectionString -> reuse existing?
        // config pr
        // new app service for SFA.DAS.EAS.Portal.Jobs (or whatever it ends up being called)
        // ^^ ConnectionStrings: AzureWebJobsDashboard, AzureWebJobsStorage
        // ^^ ApplicationSettings: ConfigurationStorageConnectionString, EnvironmentName
        // ^^ LoggingRedisConnectionString (check) LoggingRedisKey (check), StorageConnectionString (needed?)
        // ^^ APPINSIGHTS_INSTRUMENTATIONKEY?
        // CosmosDb

        static async Task Main(string[] args)
        {
            //await CreateHostBuilder(args).RunConsoleAsync();

            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();
                //https://github.com/Azure/azure-webjobs-sdk/issues/1940

                var jobHost = host.Services.GetService<IJobHost>();
                await jobHost.CallAsync(nameof(CreateReadStoreDatabaseJob.CreateReadStoreDatabase));

                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging() //todo: need to check logging/redis/use of localhost:6379 locally
                .UseApplicationInsights() // todo: need to add APPINSIGHTS_INSTRUMENTATIONKEY to config somewhere. where does it normally live? we could store it in table storage
                .UseDasEnvironment()
                .UseConsoleLifetime()
                .ConfigureServices(s => s.AddDasNServiceBus());

        //todo: need to add unit of work, config etc into container
        // does e.g. uow support non-structuremap container?
        // do we even need uow?? if we're only writing to a document collection, no sending further messages, etc.

        //.UseStructureMap()
        //.ConfigureContainer<Registry>(IoC.Initialize);
    }
}