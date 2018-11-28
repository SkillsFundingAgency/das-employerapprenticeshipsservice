using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data
{
    internal class UsersRolesRepository : DocumentRepository<UserRoles>, IUsersRolesRepository
    {
        public UsersRolesRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.UsersCollectionName)
        {
        }
    }
}