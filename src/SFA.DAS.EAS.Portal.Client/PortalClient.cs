using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;
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
        
        public Task<AccountDto> GetAccount(long accountId, CancellationToken cancellationToken = default)
        {
            return _getAccountQuery.Get(accountId, cancellationToken);
        }
    }
}