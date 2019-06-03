using System.Threading;
using System.Threading.Tasks;
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

        public Task<AccountDocument> GetAccountDocumentById(long accountId, CancellationToken cancellationToken)
        {
           return CreateQuery()
                .SingleOrDefaultAsync(a => a.AccountId == accountId, cancellationToken);
        }
    }
}