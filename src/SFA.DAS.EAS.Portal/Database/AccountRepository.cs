using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database.Models;

namespace SFA.DAS.EAS.Portal.Database
{
    public class AccountsRepository : DocumentRepository<Account>, IAccountsRepository
    {
        public AccountsRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountsCollectionName)
        {
        }
    }
}