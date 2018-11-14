using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data
{
    internal class UserRolesRepository : DocumentRepository<UserRoles>, IUsersRolesRepository
    {
        public UserRolesRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.UsersCollectionName)
        {
        }
    }
}