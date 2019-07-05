using System.Threading;
using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Database;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    public class AccountsReadOnlyRepository : ReadOnlyDocumentRepository<AccountDocument>, IAccountsReadOnlyRepository
    {
        public AccountsReadOnlyRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountsCollectionName)
        {
            
        }

        public async Task<AccountDocument> GetAccountDocumentById(long accountId, CancellationToken cancellationToken)
        {
            return await CreateQuery()
                .FirstOrDefaultAsync(a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}