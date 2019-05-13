using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;

namespace SFA.DAS.EAS.Portal.Client
{
    public interface IPortalClient
    {
        Task<AccountDto> GetAccount(long accountId, CancellationToken cancellationToken = default);
    }
}