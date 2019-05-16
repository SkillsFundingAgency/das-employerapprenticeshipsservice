using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    internal class AccountsReadOnlyRepository : ReadOnlyDocumentRepository<AccountDto>, IAccountsReadOnlyRepository
    {
        public AccountsReadOnlyRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountsCollectionName)
        {
        }
    }
}