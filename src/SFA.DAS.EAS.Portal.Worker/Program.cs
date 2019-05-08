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
        //todo: check >1 webjob
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
        //         EnvironmentName                                DEVAZURE, AT etc.
        //         ConfigurationStorageConnectionString           storage account containing config tables
        //       Connection Strings
        //         AzureWebJobsStorage                            \ storage account
        //         AzureWebJobsDashboard                          /

        //useful links
        //https://stackoverflow.com/questions/51970969/how-to-use-hostbuilder-for-webjob
        //https://github.com/Azure/azure-webjobs-sdk/wiki/Application-Insights-Integration
        //https://github.com/Azure/azure-webjobs-sdk/blob/dev/sample/SampleHost/Program.cs

        //todo: how to see & trigger individual jobs in v3?
        //https://github.com/Azure/azure-webjobs-sdk/issues/1975

        //todo: test app insights

        //todo: update sfa.das.nservicebus to use latest nservicebus.newtonsoft.json, so we can use v12 of newtonsoft.json
        
        //todo: functions instead? https://github.com/tmasternak/NServiceBus.Functions

        //todo: devops
        // config: 4 variables : branch: CON-378-EAS-Portal. pr: https://github.com/SkillsFundingAgency/das-employer-config/pull/264
        // ^^ ServiceBusConnectionString -> reuse existing?
        // EAS branch: CON-451-create-webjob-host
        // new app service for SFA.DAS.EAS.Portal.Worker (core)
        // ^^ ConnectionStrings: AzureWebJobsDashboard, AzureWebJobsStorage
        // ^^ ApplicationSettings: ConfigurationStorageConnectionString, EnvironmentName, LoggingRedisConnectionString, APPINSIGHTS_INSTRUMENTATIONKEY
        // ^^ don't think we need: LoggingRedisKey?? (check), StorageConnectionString (needed?)
        // CosmosDb

        //todo: looks like UseMessageConventions is a bit too inclusive... (consequences?)
        // better to use IEvent
        // (also would be worthwhile making sure correlationids flow)
        //2019-05-01 08:34:18.9288 [DEBUG] [NServiceBus.SerializationFeature] - Message definitions:
        //SFA.DAS.EAS.Portal.Application.Commands.AddReserveFundingCommand
        //SFA.DAS.NServiceBus.ClientOutbox.Commands.ProcessClientOutboxMessageCommand
        //NServiceBus.ScheduledTask
        //SFA.DAS.NServiceBus.Event
        //SFA.DAS.NServiceBus.Command
        //SFA.DAS.EAS.Portal.TempEvents.ReserveFundingAddedEvent

        static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();
                //https://github.com/Azure/azure-webjobs-sdk/issues/1940

                var jobHost = host.Services.GetService<IJobHost>();
                await jobHost.CallAsync(nameof(CreateReadStoreDatabaseJob.CreateReadStoreDatabase));

                await host.WaitForShutdownAsync();
            }
        }

        //todo: need to add unit of work into container?
        // does e.g. uow support non-structuremap container?
        // do we even need uow?? if we're only writing to a document collection, no sending further messages, etc.

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging() //todo: need to check logging/redis/use of localhost:6379 locally
                .UseApplicationInsights() // todo: need to add APPINSIGHTS_INSTRUMENTATIONKEY to config somewhere. where does it normally live? we could store it in table storage
                .UseDasEnvironment()
                .UseConsoleLifetime()
                .ConfigureServices(s => s.AddApplicationServices())
                .ConfigureServices(s => s.AddCosmosDatabase())
                .ConfigureServices(s => s.AddDasNServiceBus());
    }
}