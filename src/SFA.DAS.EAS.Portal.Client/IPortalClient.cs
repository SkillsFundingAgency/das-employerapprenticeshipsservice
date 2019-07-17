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
        /// <param name="parameters">Parameter object.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns></returns>
        Task<Account> GetAccount(GetAccountParameters parameters, CancellationToken cancellationToken = default);
    }
}