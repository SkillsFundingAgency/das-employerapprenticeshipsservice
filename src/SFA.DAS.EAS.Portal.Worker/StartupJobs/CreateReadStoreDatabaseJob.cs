using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Client.Data;

namespace SFA.DAS.EAS.Portal.Worker.StartupJobs
{
    /// <summary>
    /// Creates the read store as a CosmosDB if it doesn't already exist.
    /// </summary>
    /// <remarks>
    /// 
    /// UNIQUE KEYS
    /// 
    /// We don't create any unique keys, because...
    /// * Sparse unique keys are not supported.
    /// As there is no guarantee to the order we receive events from the various subsystems (due to user behaviour and
    /// out of order message handling), then we couldn't e.g. have ReservationId as an unique key,
    /// as the user might not reserve funding first.
    /// * Existing databases can't have their unique key changed.
    /// At least not without creating a new database and migrating the data across.
    /// That effectively stops us from iteratively adding unique keys and having to determine the whole schema up front.
    /// * As a bonus, it's cheaper not to have them!
    ///
    /// It does mean there's no enforcing of sensible data on our documents, especially in light of bugs in the portal
    /// or bad behaviour by event publishers (which is outside of our control) :-(
    /// We could have a data sanitiser/checker job to highlight any issues, which could be combined with the job
    /// that discards old messages (which doesn't exist yet).
    /// Possibly every event will end up creating an AccountLegalEntity, so we could have an unique index on its Id,
    /// but even if that fits current known requirements, it might not fit all future requirements.
    ///
    /// INDEXING
    ///
    /// It's tempting to set indexing to none, as we should be retrieving by accountid only,
    /// and indexes can be added on the fly if required. However, it's probably prudent to keep indexing on (everything),
    /// so that ad-hoc queries can be run (at the cost of extra storage and write latency), and also future jobs might
    /// need them.
    /// 
    /// At least the indexing policy is not set in stone at creation time, so we can change it later.
    /// </remarks>
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
            var database = await CreateDatabaseIfNotExists(executionContext, logger);

            await CreateAccountDocumentCollectionIfNotExists(database, executionContext, logger);
        }

        private async Task<Microsoft.Azure.Documents.Database> CreateDatabaseIfNotExists(ExecutionContext executionContext, ILogger logger)
        {
            var database = new Microsoft.Azure.Documents.Database
            {
                Id = DocumentSettings.DatabaseName
            };

            var createDatabaseResponse = await _documentClient.CreateDatabaseIfNotExistsAsync(database);
            logger.LogInformation($"{executionContext.InvocationId}: Create database returned {createDatabaseResponse.StatusCode}");

            return database;
        }

        //todo: namespace
        private async Task CreateAccountDocumentCollectionIfNotExists(
            Microsoft.Azure.Documents.Database database,
            ExecutionContext executionContext, ILogger logger)
        {
            var documentCollection = new DocumentCollection
            {
                Id = DocumentSettings.AccountsCollectionName,
                PartitionKey = new PartitionKeyDefinition
                {
                    Paths = new Collection<string>
                    {
                        "/accountId"
                    }
                }
            };

            var createDocumentCollectionResponse = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(database.Id),
                documentCollection,
                new RequestOptions {OfferThroughput = 1000});
            logger.LogInformation(
                $"{executionContext.InvocationId}: Create document collection returned {createDocumentCollectionResponse.StatusCode}");
        }
    }
}