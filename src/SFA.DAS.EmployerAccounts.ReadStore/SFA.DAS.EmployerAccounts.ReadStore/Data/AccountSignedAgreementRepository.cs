using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data
{
    public class AccountSignedAgreementRepository : DocumentRepository<AccountSignedAgreement>, IAccountSignedAgreementsRepository
    {
        public AccountSignedAgreementRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountUsersCollectionName)
        {
        }
    }
}