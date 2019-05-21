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

## Todo

* issue with singleton webjob in AT:
actually, it's behaving as it's been told - only 1 instance of the job at once, but each host kicks off the job
we can probably leave as is, as the webjob is idempotent, but we could clean it up so that it only runs in 1 host
[05/13/2019 13:01:23 > 04c320: INFO] 2019-05-13 13:01:23.9002 [INFO] [Function.CreateReadStoreDatabase] - Executing 'CreateReadStoreDatabaseJob.CreateReadStoreDatabase' (Reason='This function was programmatically called via the host APIs.', Id=645de859-b46f-4b8b-8d80-694f64232464) 
[05/13/2019 13:01:24 > 2b74a0: INFO] 2019-05-13 13:01:24.3533 [INFO] [Function.CreateReadStoreDatabase.User] - 27162130-b183-4482-b3c3-4d8f4e62bd06: Create database returned Created 
[05/13/2019 13:01:25 > 2b74a0: INFO] 2019-05-13 13:01:25.0122 [INFO] [Function.CreateReadStoreDatabase.User] - 27162130-b183-4482-b3c3-4d8f4e62bd06: Create document collection returned Created 
[05/13/2019 13:01:25 > 2b74a0: INFO] 2019-05-13 13:01:25.0567 [INFO] [Function.CreateReadStoreDatabase] - Executed 'CreateReadStoreDatabaseJob.CreateReadStoreDatabase' (Succeeded, Id=27162130-b183-4482-b3c3-4d8f4e62bd06) 
[05/13/2019 13:01:25 > 2b74a0: INFO] 2019-05-13 13:01:25.0567 [INFO] [Host.Results] -  
[05/13/2019 13:01:29 > 04c320: INFO] 2019-05-13 13:01:29.8007 [INFO] [Function.CreateReadStoreDatabase.User] - 645de859-b46f-4b8b-8d80-694f64232464: Create database returned OK 
[05/13/2019 13:01:29 > 04c320: INFO] 2019-05-13 13:01:29.9726 [INFO] [Function.CreateReadStoreDatabase.User] - 645de859-b46f-4b8b-8d80-694f64232464: Create document collection returned OK 
[05/13/2019 13:01:30 > 04c320: INFO] 2019-05-13 13:01:30.0250 [INFO] [Function.CreateReadStoreDatabase] - Executed 'CreateReadStoreDatabaseJob.CreateReadStoreDatabase' (Succeeded, Id=645de859-b46f-4b8b-8d80-694f64232464) 

* create readme.md for portal

* it would be better to have a webjob to periodically clean-up processed messages

* unit tests for infrastructure

* how to see & trigger individual jobs in v3?
https://github.com/Azure/azure-webjobs-sdk/issues/1975

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

