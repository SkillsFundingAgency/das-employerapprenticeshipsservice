using Microsoft.Azure.Documents;

namespace SFA.DAS.EAS.Portal.Database
{
    public interface IDocumentClientFactory
    {
        IDocumentClient CreateDocumentClient();
    }
}
