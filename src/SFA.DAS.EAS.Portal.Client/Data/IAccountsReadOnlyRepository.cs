using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    internal interface IAccountsReadOnlyRepository : IReadOnlyDocumentRepository<AccountDto>
    {
    }
}