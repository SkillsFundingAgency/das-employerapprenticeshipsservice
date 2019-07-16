using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client
{
    public interface IPortalClient
    {
        /// <summary>
        /// Get the account details required to render the homepage.
        /// </summary>
        /// <param name="hashedAccountId">The (non-public) hashed account id.</param>
        /// <param name="accountState">The current state of the account. Set all flags that apply.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns></returns>
        Task<Account> GetAccount(string hashedAccountId, AccountState accountState,
            CancellationToken cancellationToken = default);
    }
}