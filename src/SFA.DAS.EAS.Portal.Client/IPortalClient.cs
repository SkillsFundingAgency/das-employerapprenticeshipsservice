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
        /// <param name="maxNumberOfVacancies">The maximum number of vacancies to return.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns></returns>
        Task<Account> GetAccount(string hashedAccountId, int maxNumberOfVacancies,
            CancellationToken cancellationToken = default);
    }
}