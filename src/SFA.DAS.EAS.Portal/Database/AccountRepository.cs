using Microsoft.Azure.Documents;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Database.Models;

namespace SFA.DAS.EAS.Portal.Database
{
    public class AccountsRepository : EnhancedDocumentRepository<Account, long>, IAccountsRepository
    {
        public AccountsRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountsCollectionName)
        {
        }
    }
}