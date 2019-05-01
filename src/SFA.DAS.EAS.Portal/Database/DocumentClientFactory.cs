using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.EAS.Portal.Configuration;

namespace SFA.DAS.EAS.Portal.Database
{
    public class DocumentClientFactory : IDocumentClientFactory
    {
        private readonly CosmosDatabaseConfiguration _configuration;

        public DocumentClientFactory(CosmosDatabaseConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDocumentClient CreateDocumentClient()
        {
            var connectionPolicy = new ConnectionPolicy
            {
                RetryOptions =
                {
                    MaxRetryAttemptsOnThrottledRequests = 4,
                    MaxRetryWaitTimeInSeconds = 10
                }
            };

            return new DocumentClient(new Uri(_configuration.Uri), _configuration.AuthKey, connectionPolicy);
        }
    }
}
