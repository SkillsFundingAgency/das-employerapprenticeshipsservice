﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
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

            var documentCollections = new List<DocumentCollection>
            {
                new DocumentCollection
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
                                Paths = new Collection<string> {"/userRef"}
                            }
                        }
                    }
                },
                new DocumentCollection
                {
                    Id = DocumentSettings.AccountSignedAgreementsCollectionName,
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
                                Paths = new Collection<string>
                                {
                                    "/accountId",
                                    "/agreementVersion",
                                    "/agreementType"
                                }
                            }
                        }
                    }
                }
            };

            _logger.LogInformation("Creating ReadStore database and collection if they don't exist");

            var createDatabaseResponse = await _documentClient.CreateDatabaseIfNotExistsAsync(database);
            _logger.LogInformation($"Database {(createDatabaseResponse.StatusCode == HttpStatusCode.Created ? "created" : "already existed")}");

            var requestOptions = new RequestOptions { OfferThroughput = 1000 };
            foreach (var documentCollection in documentCollections)
            {
                var createDocumentCollectionResponse = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(database.Id), documentCollection, requestOptions);
                _logger.LogInformation($"Document collection {(createDocumentCollectionResponse.StatusCode == HttpStatusCode.Created ? "created" : "already existed")}");
            }
        }
    }
}