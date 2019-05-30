using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Database.Models;

namespace SFA.DAS.EAS.Portal.Database
{
    public interface IAccountsRepository : IDocumentRepository<AccountDocument>
    {
    }
}