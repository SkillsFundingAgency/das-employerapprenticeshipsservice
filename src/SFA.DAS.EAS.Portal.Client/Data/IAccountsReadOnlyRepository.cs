using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    internal interface IAccountsReadOnlyRepository : IReadOnlyDocumentRepository<AccountDocument>
    {
        Task<Account> Get(long accountId, CancellationToken cancellationToken = default);
    }
}