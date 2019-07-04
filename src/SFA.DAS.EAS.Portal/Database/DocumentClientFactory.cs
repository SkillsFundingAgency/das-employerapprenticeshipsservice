using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Portal.Infrastructure.Configuration;

namespace SFA.DAS.EAS.Portal.Database
{
    public class DocumentClientFactory : IDocumentClientFactory
    {
        private readonly IOptionsMonitor<CosmosDatabaseConfiguration> _options;

        public DocumentClientFactory(IOptionsMonitor<CosmosDatabaseConfiguration> options)
        {
            _options = options;
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

            var currentOptions = _options.CurrentValue;

            return new DocumentClient(new Uri(currentOptions.Uri), currentOptions.AuthKey, connectionPolicy);
        }
    }
}
