using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Models;

namespace SFA.DAS.EAS.Portal.Client
{
    public class PortalClient : IPortalClient
    {
        private readonly GetAccountQuery _getAccountQuery;

        //todo: think through di lifetime, ie transient vs singleton. if we can singleton all the way, wahey
        public PortalClient(GetAccountQuery getAccountQuery)
        {
            _getAccountQuery = getAccountQuery;
        }

        public Task<IAccountDto<IAccountLegalEntityDto<IReservedFundingDto>>> GetAccount(long accountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _getAccountQuery.Get(accountId, cancellationToken);
        }
    }
}