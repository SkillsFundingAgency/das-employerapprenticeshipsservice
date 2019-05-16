using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Types;

namespace SFA.DAS.EAS.Portal.Client
{
    public interface IPortalClient
    {
        Task<Account> GetAccount(long accountId, CancellationToken cancellationToken = default);
    }
}