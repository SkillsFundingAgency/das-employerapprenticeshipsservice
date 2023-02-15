using System.Collections.ObjectModel;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.Jobs.StartupJobs;

public class CreateReadStoreDatabaseJob
{
    private readonly IDocumentClient _documentClient;
    private readonly ILogger<CreateReadStoreDatabaseJob> _logger;

    public CreateReadStoreDatabaseJob(IDocumentClient documentClient, ILogger<CreateReadStoreDatabaseJob> logger)
    {
        _documentClient = documentClient;
        _logger = logger;
    }

    [NoAutomaticTrigger]
    public async Task Run()
    {
        var database = new Database
        {
            Id = DocumentSettings.DatabaseName
        };

        var documentCollection = new DocumentCollection
        {
            Id = DocumentSettings.AccountUsersCollectionName,
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
                    new UniqueKey
                    {
                        Paths = new Collection<string> { "/userRef" }
                    }
                }
            }
        };

        _logger.LogInformation("Creating ReadStore database and collection if they don't exist");

        var createDatabaseResponse = await _documentClient.CreateDatabaseIfNotExistsAsync(database);
        _logger.LogInformation($"Database {(createDatabaseResponse.StatusCode == HttpStatusCode.Created ? "created" : "already existed")}");

        var requestOptions = new RequestOptions { OfferThroughput = 1000 };
        var createDocumentCollectionResponse = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(database.Id), documentCollection, requestOptions);
        _logger.LogInformation($"Document collection {(createDocumentCollectionResponse.StatusCode == HttpStatusCode.Created ? "created" : "already existed")}");
    }
}