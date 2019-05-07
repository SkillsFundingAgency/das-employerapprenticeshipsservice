using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.CosmosDb;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    public class DocumentClientFactory : IDocumentClientFactory
    {
        private readonly ICosmosDbConfiguration _configuration;

        public DocumentClientFactory(ICosmosDbConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDocumentClient CreateDocumentClient()
        {
            var connectionPolicy = new ConnectionPolicy
            {
                RetryOptions =
                {
                    MaxRetryAttemptsOnThrottledRequests = 2,
                    MaxRetryWaitTimeInSeconds = 2
                }
            };

            return new DocumentClient(new Uri(_configuration.Uri), _configuration.AuthKey, connectionPolicy);
        }
    }
}