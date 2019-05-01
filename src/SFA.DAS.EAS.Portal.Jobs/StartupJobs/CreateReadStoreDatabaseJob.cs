using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Database;

namespace SFA.DAS.EAS.Portal.Jobs.StartupJobs
{
    public class CreateReadStoreDatabaseJob
    {
        private readonly IDocumentClient _documentClient;

        public CreateReadStoreDatabaseJob(IDocumentClient documentClient)
        {
            _documentClient = documentClient;
        }

        // singleton attribute requires a real storage account, so webjobs has access to blobs (so emulator doesn't work)
        // set env variables AzureWebJobsDashboard & AzureWebJobsStorage (for now) to a real storage account
        // add to readme.md?
        // emulator support blobs, but this still doesn't work.why?
        //todo: use secret manager, rather than env variables (easier to have different settings for different projects)
        // ^^ see https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
        //todo: inject ilogger in ctor?
        [NoAutomaticTrigger]
        [Singleton]
        public async Task CreateReadStoreDatabase(ExecutionContext executionContext, ILogger logger)
        {
            //logger.LogInformation($"{executionContext.InvocationId}: Helo Byd");

            var database = new Microsoft.Azure.Documents.Database
            {
                Id = DocumentSettings.DatabaseName
            };

            var documentCollection = new DocumentCollection
            {
                Id = DocumentSettings.AccountsCollectionName,
                PartitionKey = new PartitionKeyDefinition
                {
                    Paths = new Collection<string>
                    {
                        "/accountId"
                    }
                },
                UniqueKeyPolicy = new UniqueKeyPolicy
                {
                    UniqueKeys = new Collection<UniqueKey>
                    {
                        //new UniqueKey
                        //{
                        //    Paths = new Collection<string> { "/accountProviderLegalEntityId" }
                        //}
                    }
                }
            };

            logger.LogInformation($"{executionContext.InvocationId}: Creating database and collection if they don't exist");

            var createDatabaseResponse = await _documentClient.CreateDatabaseIfNotExistsAsync(database);
            logger.LogInformation($"{executionContext.InvocationId}: Create database returned {createDatabaseResponse.StatusCode}");

            var createDocumentCollectionResponse = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(database.Id),
                documentCollection,
                new RequestOptions { OfferThroughput = 1000 });
            logger.LogInformation($"{executionContext.InvocationId}: Create document collection returned {createDocumentCollectionResponse.StatusCode}");
        }
    }
}