# Dev Notes

 to run
   local
     set environment variables
       ASPNETCORE_ENVIRONMENT                            Development (currently set in launchsettings.json)
       APPSETTING_ConfigurationStorageConnectionString   storage account containing config tables (can be emulator)
       AzureWebJobsStorage                               \ real storage account (not emulator)
       AzureWebJobsDashboard                             /
       todo: ^ ConnectionStrings: prefix??

    note: Config TableStorage PartitionKey locally must be `Development`, rather than `LOCAL`

   real published app service
       Application Settings
         ASPNETCORE_ENVIRONMENT                         DEVAZURE, AT etc.
         ConfigurationStorageConnectionString           storage account containing config tables
       Connection Strings
         AzureWebJobsStorage                            \ storage account
         AzureWebJobsDashboard                          /

devops notes
 config: 4 variables : branch: CON-378-EAS-Portal. pr: https://github.com/SkillsFundingAgency/das-employer-config/pull/264
 ^^ ServiceBusConnectionString -> reuse existing?
 EAS branch: CON-451-create-webjob-host
 new app service for SFA.DAS.EAS.Portal.Worker (core)
 ^^ ConnectionStrings: AzureWebJobsDashboard, AzureWebJobsStorage
 ^^ ApplicationSettings: ConfigurationStorageConnectionString, EnvironmentName, LoggingRedisConnectionString, APPINSIGHTS_INSTRUMENTATIONKEY
 ^^ don't think we need: LoggingRedisKey?? (check), StorageConnectionString (needed?)
 CosmosDb

## Todo

* create readme.md for portal

* how to see & trigger individual jobs in v3?
https://github.com/Azure/azure-webjobs-sdk/issues/1975

* test app insights

* update sfa.das.nservicebus to use latest nservicebus.newtonsoft.json, so we can use v12 of newtonsoft.json

* functions instead? https://github.com/tmasternak/NServiceBus.Functions

* looks like UseMessageConventions is a bit too inclusive... (consequences?)
 better to use IEvent
 (also would be worthwhile making sure correlationids flow)
2019-05-01 08:34:18.9288 [DEBUG] [NServiceBus.SerializationFeature] - Message definitions:
SFA.DAS.EAS.Portal.Application.Commands.AddReservationCommand
SFA.DAS.NServiceBus.ClientOutbox.Commands.ProcessClientOutboxMessageCommand
NServiceBus.ScheduledTask
SFA.DAS.NServiceBus.Event
SFA.DAS.NServiceBus.Command
SFA.DAS.EAS.Portal.TempEvents.ReservationCreatedEvent

