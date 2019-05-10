using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database.Models;

namespace SFA.DAS.EAS.Portal.Database
{
    public interface IAccountsRepository : IEnhancedDocumentRepository<Account, long>
    {
    }
}