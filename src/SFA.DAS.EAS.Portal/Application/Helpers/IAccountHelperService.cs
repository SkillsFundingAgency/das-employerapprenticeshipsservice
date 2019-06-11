using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.AccountHelper
{
    public interface IAccountHelperService
    {
        Task<AccountDocument> GetOrCreateAccount(long accountId, CancellationToken cancellationToken = default);
    }
}