using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Data
{
    internal interface IUsersRolesRepository : IDocumentRepository<UserRoles>
    {
    }
}