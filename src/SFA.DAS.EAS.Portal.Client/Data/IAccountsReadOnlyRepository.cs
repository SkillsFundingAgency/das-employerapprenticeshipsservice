using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    //todo: internal
    public interface IAccountsReadOnlyRepository : IReadOnlyDocumentRepository<AccountDto>
    {
    }
}