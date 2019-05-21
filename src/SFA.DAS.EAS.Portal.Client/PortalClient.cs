using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Types;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client
{
    public class PortalClient : IPortalClient
    {
        private readonly GetAccountQuery _getAccountQuery;

        public PortalClient(IContainer container)
        {
            _getAccountQuery = container.GetInstance<GetAccountQuery>();
        }
        
        public Task<Account> GetAccount(long accountId, CancellationToken cancellationToken = default)
        {
            return _getAccountQuery.Get(accountId, cancellationToken);
        }
    }
}