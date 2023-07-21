using Microsoft.Azure.Documents;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data;

public interface IDocumentClientFactory
{
    IDocumentClient CreateDocumentClient();
}