using Microsoft.Azure.Documents;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    public interface IDocumentClientFactory
    {
        IDocumentClient CreateDocumentClient();
    }
}