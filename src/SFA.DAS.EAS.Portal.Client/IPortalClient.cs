using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client
{
    public interface IPortalClient
    {
        Task<Account> GetAccount(long accountId, string publicHashedAccountId,
            AccountState accountState, CancellationToken cancellationToken = default);
    }
}