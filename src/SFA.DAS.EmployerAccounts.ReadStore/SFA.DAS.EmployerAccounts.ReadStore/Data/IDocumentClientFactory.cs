using Microsoft.Azure.Documents;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data;

internal interface IDocumentClientFactory
{
    IDocumentClient CreateDocumentClient();
}