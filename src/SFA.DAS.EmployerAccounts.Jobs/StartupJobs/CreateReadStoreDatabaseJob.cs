using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.Jobs.StartupJobs
{
    public class CreateReadStoreDatabaseJob
    {
        private readonly IDocumentClient _documentClient;
        private readonly ILogger _logger;

        public CreateReadStoreDatabaseJob(IDocumentClient documentClient, ILogger logger)
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

            await _documentClient.CreateDatabaseIfNotExistsAsync(database);
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(database.Id), documentCollection);
        }
    }
}