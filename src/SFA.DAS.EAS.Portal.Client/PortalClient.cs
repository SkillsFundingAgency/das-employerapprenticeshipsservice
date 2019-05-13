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

        //todo: could support structuremap & core DI in same client, but would be better to have separate client assemblies to keep the entourage down
        public PortalClient(IContainer container)
        {
            _getAccountQuery = container.GetInstance<GetAccountQuery>();
        }

//        public PortalClient(IServicepProvider container)
//        {
//        }
        
        public Task<AccountDto> GetAccount(long accountId, CancellationToken cancellationToken = default)
        {
            return _getAccountQuery.Get(accountId, cancellationToken);
        }
    }
}