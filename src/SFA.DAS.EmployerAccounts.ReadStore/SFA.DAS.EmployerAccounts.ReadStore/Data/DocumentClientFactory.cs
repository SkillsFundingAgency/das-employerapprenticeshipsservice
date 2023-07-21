using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data;

public class DocumentClientFactory : IDocumentClientFactory
{
    private readonly EmployerAccountsReadStoreConfiguration _configuration;

    public DocumentClientFactory(EmployerAccountsReadStoreConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDocumentClient CreateDocumentClient()
    {
        var connectionPolicy = new ConnectionPolicy
        {
            RetryOptions =
            {
                MaxRetryAttemptsOnThrottledRequests = 3,
                MaxRetryWaitTimeInSeconds = 2
            }
        };

        return new DocumentClient(new Uri(_configuration.Uri), _configuration.AuthKey, connectionPolicy);
    }
}