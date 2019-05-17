using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Database;
using SFA.DAS.EAS.Portal.Client.Database.Models;

namespace SFA.DAS.EAS.Portal.Database
{
    public class AccountsRepository : DocumentRepository<AccountDocument>, IAccountsRepository
    {
        public AccountsRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountsCollectionName)
        {
        }
    }
}