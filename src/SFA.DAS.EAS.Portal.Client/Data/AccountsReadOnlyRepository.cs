using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Database;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    internal class AccountsReadOnlyRepository : ReadOnlyDocumentRepository<AccountDocument>, IAccountsReadOnlyRepository
    {
        public AccountsReadOnlyRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountsCollectionName)
        {
        }

        public async Task<Account> Get(long accountId, CancellationToken cancellationToken = default)
        {
            var document = await CreateQuery()
                .FirstOrDefaultAsync( a => a.Deleted == null && a.AccountId == accountId, cancellationToken)
                .ConfigureAwait(false);

            return document?.Account;
        }
    }
}