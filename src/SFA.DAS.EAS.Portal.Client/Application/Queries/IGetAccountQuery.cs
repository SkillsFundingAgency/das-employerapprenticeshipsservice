using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    public interface IGetAccountQuery
    {
        Task<Account> Get(long accountId, CancellationToken cancellationToken = default);
    }
}